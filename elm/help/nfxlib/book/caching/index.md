# Big Memory and Caching

Managed runtimes can not handle more than 10 million objects without pauses for garbage collection. 
Sometimes even 10 MM resident objects start to pause process. GC Heap defragmentation kills the application by adding unpredictable stalls. 
The addition of physical RAM leads to increase of unpredictability even more as GC can now postpone its full sweep. 
So it is practically impossible to use in 64/128/256GB RAM nowadays, occupied by small CLR objects - the process just stalls.

All of the modern techniques (concurrent, background, parallel, server mode) try to overcome this situation but without any significant success. 
CLR objects consume lots of RAM, for example a string “ABC” holds around 30 bytes instead of 3. 
Java has the same issue, in spite of the fact that its VMs have way more options to control GC, but it is not enough for such big memory anyway. 
The root of the problem: the GC “sees” references and has to traverse them to see what is still reachable and what is not, obviously it takes time and slows down the application.

Many resident data objects are needed for different types of applications such as neural networks, caches, pre-computed data, etc. 
Stateless design is not applicable, actually it is not possible to keep all data in database because it is very slow and it is needed to cache data right in-process anyhow. 
It it is impossible to postpone garbage collection for a long time. 

## Contents

* [Pile](./pile.md)

* [Cache](./cache.md)