# Cache

While Pile provides a lower-level services of c-like `malloc()`, `PileCache` is build on top of Pile to efficiently store a key/value database in-process.
The cache consists of named tables each having a `TKey` generic argument.
This is important as we do not want to use boxing for keys as this would defeat the purpose of having Pile altogether.
Cache is speculative, that is: it does not guarantee that you get from if what you have written. 
It is a hashtable with a linear array buffer without relocating rehashing (it does do linear probing). 
The cache keeps a load factor (configurable) that keeps it at a steady hit rate. Entries have priorities which can preclude existing item eviction (i.e. a "gold" customer object may have a higher priority than a "silver" one).
Cache supports sliding and absolute expirations which are serviced by a dedicated sweep thread.
Both Cache and Pile come with a complete instrumentation suite (gauges).
