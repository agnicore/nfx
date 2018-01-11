# Pile

**NFX** solves the problem by  "hiding" data from GC.
Until recently there wasn’t any implementation of this concept. 
We did it in .NET. and called this technology Pile. 
Pile is a custom memory manager and it has 100%-managed code. 
Pile can store native CLR objects in a tightly-serialized form and free the managed garbage collector from having to deal with this objects. 
In addition piles can be either local (allocate local RAM on the server), or distributed (allocate RAM on many servers).
Contrary to many concerns of performance degradation due to serialization, Pile yields better results in terms of both time and space than using out-of-process serialization (i.e. Redis/Memcache).

The main ideas of Pile are

* To hold objects in huge byte arrays, so GC does not “see” them.

* To use data teleportation through Slim Serializer (25% compression over CLR can be achieved).

* To turn CLR object into struct{int,int} (segment and address within Pile) and back.

* To manage “unmanaged” managed memory

Pile allocates memory and returns PilePointer structure to the caller:

```csharp
public struct PilePointer : IEquatable<PilePointer>
{
  // Distributed Node ID. The local pile sets this to -1 rendering
  // this pointer as !DistributedValid
  public readonly int NodeID;

  // Segment # within pile
  public readonly int Segment;

  // Address within the segment (offset)
  public readonly int Address;
}
```

PilePointer represents a pointer to the pile object (object stored in a pile). 
The reference may be local or distributed in which case the NodeID is >= 0.
Distributed pointers are very useful for organizing piles of objects distributed among many servers, for example  for "Big Memory" implementations or large neural networks where nodes may inter-connect between servers. 
The CLR reference to the IPile is not a part of this struct for performance and practicality reasons, as  it is highly unlikely that there are going to be more than one instance of a pile in a process, however should more than one pile be allocated than this pointer would need to be wrapped in some other structure along with source `IPile` reference.

With Pile it is possible to easily store 200,000,000 "hot" social connections with ability to traverse them in <1ms by a single thread on a 64GB box. 
Therefore most of our applications turned from IO bound to CPU bound (pile teleportation) due to all required for application data are kept in RAM. 
Another benefit we get with using Pile is Cache which is described below. 
We can cache most of DB (except for financial rollups). Pile is very comfortable for Event Sourcing. 
We do not need to make any socket calls to Redis/Memcached because we already have everything that is needed  in memory, we don’t even need IPC/domain sockets. 
Pile-based applications tend to promote "stateful" paradigm and avoid talking to data layers at all, 95% requests are served from RAM. 
Important note:  we do not need to invent special Data Transfer Objects, as pile/cache stores Rows (along with logic) - this is classical OOP: data is here, it has methods and it has state. 
It allows making projects through Actor-based programming concept - Pile pointer becomes an Actor token. 
In spite of teleportation involved, Pile yields far faster throughput and better latency had we used plain CLR native objects that would have just stalled the process due to GC interruptions. 
In addition `NFX.ApplicationModel.Pile.Cache` that bases on Pile has expiration, priority, low/high watermarks.

We allocate a large byte array and then we sub-allocate chunks of that byte array and we hand it off to the user. 
So the user of the pile comes in and says that he wants to stick in the pile some object. 
This object can be any object, it does not have to be decorated, it can be tuple of strings or can be a dictionary of strings, list of strings and etc. 
Memory manager finds the space and it squeezes user’s object there but first it serializes the object to an array of bytes and then it stores that array of bytes in the pile and return to user pile pointer. 
When it is needed user can dereference the pile pointer back to the original object. 
Then memory manager takes the bytes from address within segment and deserializes it and give the original object back. 
It is can be thought of pile as a special kind of database that it is run within the process. 
Because of Slim serializer compresses the object (adaptive compression that uses variable encoding for integers and integer arrays and UTF8 strings, 
gets rid of machine word alignment padding) so that chunk within the segment is usually less in size (in bytes size) than the object in memory (every CLR object has header with flags for a walking, 
different GC flags and all of this kinda stuff and it stores information about fields). 
The cost of this compression is almost nonexistent because it is very fast.

When some object is deallocated from the pile corresponding block gets written back to the free buffer list. 
So every segment has a registry for the free buffers where it keeps the indexes of what is free. 
That does not induce any pressure on GC because everything happens internally. 
When it is needed to allocate some new object memory manager uses those free indexes to find the most appropriate slot. 
If it can find such slot it just returns it, occupies it and writes little header that the slot is taken. 
If there is no space in indices (it may depend on the situation with multi-threading contention/too many alloc/deallocs) everything go to the next segment and try to find space there. 
After it reaches some threshold it starts crawling the segment so that the crawl operation will visit all of the holes in the segment. 
So there is fragmentation possible but in the practical world this fragmentation is acceptable because you delete some information then you repopulate and you have some free chunks (similar to a hard drive). 

In **NFX** the basis of Pile objects is `IPile` interface. 
Implementors of this interface are custom memory managers that favor the GC performance in apps with many objects at the cost of higher CPU usage. 
The implementor must be thread-safe for all operations unless stated otherwise on a member level. 
The memory represented by this class as a whole is not synchronizable, that is - it does not support functions like Interlocked-family, Lock, MemoryBarriers and the like that regular RAM supports. 
Should a need arise to interlock within the pile - a custom CLR-based lock must be used to synchronize access to pile as a whole, for example: a Get does not impose a lock on all concurrent writes through the pile (a write does not block all gets either). 

`IPile` interface has the following main methods:

* `Put` puts a CLR object into the pile and returns a newly-allocated pointer. Throws out-of-space exception if there is not enough space in the pile and limits are set. Optional lifeSpanSec parameter will auto-delete object after the interval elapses if the pile supports data expiration.
* `Get` returns a CLR object by its pile pointer or throws access violation if pointer is invalid.
* `Delete` deletes object from pile by its pointer returning true if there is no access violation and pointer is pointing to the valid object, throws otherwise unless throwInvalid parameter is set to false (in many cases it is better to return bool result than throw slow exception). 
* `Rejuvenate` if pile supports expiration, resets object age to zero, returns true if object was found and reset.
* `Purge` deletes all objects freeing all segment memory buffers. This method may require the caller to have special rights.
* `Compact` tries to delete extra capacity which is allocated but not currently needed.

Implementation of this interface is `DefaultPile`. 
It has very extensive instrumentation like object count, segment count, allocated memory bytes, utilized bytes and all of that stuff. 
Also you can tune the pile to your particular case with putting your settings for the pile to configurations file.
Pile can be run on a web server, on a multi-threaded web-server with task parallel library with async/await. 
The Pile does not expose any asynchronous APIs and that is done on purpose because the pile usually works in not an IO- but in CPU-bound operations space, 
it does a lot of memory, a lot of CPU operations, and asynchronous APIs are not necessary.

**Example:**

```csharp
using (var pile = new DefaultPile())
{
  pile.Start();
  
  string str = "Testing Pile";
  int i = 1002;

  var pp1 = pile.Put(str);
  var pp2 = pile.Put(i);

  var del = pile.Delete(pp1)      // true
  // var strNew = pile.Get(pp1);  - access violation
  var iNew = pile.Get(pp2);       // 1002
}
```