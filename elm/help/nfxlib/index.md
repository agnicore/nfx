# NFX

**NFX** is a modern .NET full stack framework designed for building cloud and on-premises apps.
It is written in C# and runs on a CLR machine. 
**NFX** supports app containers, configuration, big memory heaps, IPC, and functions that significantly simplify 
the development of large distributed systems. It boosts performance and simplifies the development (such as services/web). 
The majority of the achievements are possible because of the following key features:

* Unification of design - all components are written in the same way

* Sophisticated serialization mechanism - moves objects between distributed processes/nodes (aka "teleportation")

* Object Pile - a 100%-managed "Big Memory" approach for utilization of hundreds of gigabytes of RAM without GC stalls

### promoting:

* Distributed systems: clusters/hierarchies

* IPC - contract-based object-oriented high performance services

* Stateful/stateless programming (web/service)

* Full utilization of big RAM capacities in-proc (i.e. 128 Gb resident) without GC stalls 
  
###good for:

* Linux-style prоgramming in .NET. Minimalistic, elegant

* Vendor de-coupling 

* General Scalability (i.e. hybrid DataStores with virtual queries and CQRS)

* Distributed Macro/micro/nano services, REST or RPC

* Actor-model systems with message passing and supervisors

* In-memory processing, for hundreds of millions of objects in-RAM with full GC < 20ms

* Supercomputer/Cluster/Farm applications (i.e. bulk rendering cluster, social graphs etc.)

* High-performance business apps (i.e. serving 50,000+ BUSINESS WEB requests a second (with logic) on a general server 
  looking up data in a 300,000,000 business object cache in-RAM)
  
* non-trivial CLR cases like: structs with read-only fields, arrays of structs of structs, custom streamers like Dictionary<> with comparers etc. In the past 7 years, teleportation mechanism has moved trillions of various CLR object instances

License: Apache 2.0

<br>

<h2 id="contribute">
    <a href="https://github.com/agnicore/nfx" class="extLink" target="_blank">
       <img src="images/3rdparties/github.svg">
       Get The Code, Contribute
    </a>
</h2>

<h2 id="install">
    <a href="https://www.nuget.org/packages/NFX" class="extLink" target="_blank">
       <img src="images/3rdparties/nuget.svg">
       Install instructions
    </a>
</h2>

<h2 id="vidoes">
    <a href="https://www.youtube.com/user/itadapterinc/videos" class="extLink" target="_blank">
       <img src="images/3rdparties/youtube.svg">
       Watch Our Videos
    </a>
</h2>

   * <a href="https://www.youtube.com/user/itadapterinc/videos" target="_blank">Main channel</a>
   * <a href="https://www.youtube.com/channel/UCKv4mLAN-XjZF2ST0cT6pAQ" target="_blank">Some tutorials(rus)</a>
   * <a href="https://www.youtube.com/watch?v=BdDlRRADlgk" target="_blank">.Net Big Memory(rus)</a>

## Documentation - Read the Manual browse Docs
  
  * [Book](./book/index.md)
  * [Specifications](./specs/index.md)
  * [Tutorials](./tutorials/index.md)
  * [Tools](./tools/index.md)
  * [Code Documentation](./docs/index.md)
  * [Archive](./archive/blog/index.md)

## The Press
  
   * <a href="https://www.infoq.com/articles/Big-Memory-Part-1" target="_blank">Big Memory .NET Part 1 - The Challenges in Handling 1 Billion Resident Business Objects</a>
   * <a href="https://www.infoq.com/articles/Big-Memory-Part-2" target="_blank">Big Memory .NET Part 2 - Pile, Our Big Memory Solution for .NET</a>
   * <a href="https://www.infoq.com/articles/Big-Memory-Part-3" target="_blank">Unleashing the Power of .NET Big Memory and Memory Mapped Files</a>
 
## Demos
   
   * <a href="https://github.com/aumcode/nfx-demos" target="_blank">All</a>
   * <a href="https://github.com/aumcode/nfx-demos/tree/master/Config" target="_blank">Config</a>
   * <a href="https://github.com/aumcode/nfx-demos/tree/master/Wave" target="_blank">Wave</a>
   * <a href="https://github.com/aumcode/nfx-demos/tree/master/Glue" target="_blank">Glue</a>
   * <a href="http://aumcode.github.io/serbench" target="_blank">Slim Serializer Benchmarks</a>