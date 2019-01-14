

# The NFX project has been [superseded by Azos Project https://github.com/azist/azos](https://github.com/azist/azos)

##  ~~NFX - .NET Standard Unistack Framework~~

<br><br><br><br><br><br><br>

### ~~NFXv5 ANNOUNCEMENT~~
The current development of NFX **HAS MOVED HERE** ~~is moving~~. This repository contains the current NFX (v5) project which targets .NET 4.7.1 and Net Stadard 2/ Core 2. We have moved from [legacy NFX v3 repository](https://github.com/aumcode/nfx)

**NFX**(v5 and beyond) **is based on .Net Standard 2**,
having all of cli tools targeting both .NET FX 4.7.1+ and .NET Core 2+.
Future NFX will not officially support targets below .NET 4.7.1 (though possible) and .NET Core 2.

The legacy [NFXv3* repo](https://github.com/aumcode/nfx) will continue to target .NET 4.5/Mono and may be supported for major bug fixes however **all new feature development is going to happen here in NFXv5+**.

<img src="https://github.com/agnicore/nfx/blob/master/elm/logo/New.NFX.Logo.50.png" alt="Logo">

Server **UNISTACK** *(unified full software stack avoiding dependencies on 3rd party libs)* framework. 

License: **Apache 2.0**   Runs:  **.NET 4.7.1+**, **Core 2+** 

[<img src="https://ci.appveyor.com/api/projects/status/6qhr08iclbbfc5f5?svg=true" alt="Project Badge" width="200">](https://ci.appveyor.com/project/itadapter/nfx-29btl/history)

Documentation: [http://nfxlib.com](http://nfxlib.com)

## Framework/Platform Support
NFX is now based on .NET Standard which works on different runtimes. Officially we support .NET Core and .NET Framework:
* **.NET Standard 2** - supported
* **.NET Core 2** - supported
* **.NET Framework 4.7.1** - supports classic .NET Framework 4.7.1+

NFX supports **cross-platform development** and is tested on:
* **Windows** Win 2010 Core 2 and Net Fx
* **Linux** Ubuntu 16 LTS using Core 2
* **Mac** OS 14 using Core 2

NFX Builds on:
* **Windows** / MSBuild 15 / VS 2017
* in process - *nix *(need scripts and build process refinements)*

NFX abstacts platform-specific functions via **PAL** *(Platform Abstraction Layer)* which implements features such as: 2D graphics (drawing and image compression), CPU/RAM performance counters acquisition, Security functions (EnsureAccessibleDirectory etc.). PAL is implemented for every runtime differently and gets injected into NFX at process entry point.

All tools get compiled in .NET Core *(out/{Config}/run-core)* and .NET Framework
*(out/{Config}/run-netf)* variants all targeting the same NFX.dll which is linked against .NET Standard 2. 

## About NFX

NFX is a modern .NET full stack framework designed for building **cloud** and **on-premises** apps.
It is written in C# and runs on a CLR machine linking against .NET Stadard 2. 
NFX supports app containers, configuration, big memory heaps, IPC, and functions that significantly simplify 
the development of large distributed systems. It boosts performance and simplifies the development (such as services/web). 
The majority of the achievements are possible because of the following key features:

* Unification of design - all components are written in the same way
* Sophisticated serialization mechanism - moves objects between distributed processes/nodes (aka "teleportation")
* Object Pile - a [**100%-managed "Big Memory" approach for utilization of hundreds of gigabytes of RAM**](./src/NFX/ApplicationModel/Pile) without GC stalls

promoting:

* Distributed systems: clusters/hierarchies
* IPC - contract-based object-oriented high performance services
* Stateful/stateless programming (web/service)
* Full utilization of big RAM capacities in-proc (i.e. 128 Gb resident) without GC stalls 
  
good for:

* Linux-style prоgramming in .NET. Minimalistic, elegant.
* Vendor de-coupling 
* General Scalability (i.e. hybrid DataStores with virtual queries and CQRS)
* Distributed Macro/micro/nano services, REST or RPC
* Actor-model systems with message passing and supervisors
* [**In-memory processing**, for **hundreds of millions of objects** in-RAM with full GC < 20ms](./src/NFX/ApplicationModel/Pile)
* **Supercomputer/Cluster/Farm** applications (i.e. bulk rendering cluster, social graphs etc.)
* **High-performance business** apps (i.e. serving 50,000+ BUSINESS WEB requests a second (with logic) on a general server 
  looking up data in a [300,000,000 business object cache in-RAM](./src/NFX/ApplicationModel/Pile))
* non-trivial CLR cases like: structs with read-only fields, arrays of structs of structs, custom streamers like Dictionary<> with comparers etc. In the past 7 years, teleportation mechanism has moved trillions of various CLR object instances


## Guides/Documentation
All Guides and Docs/Samples/Tutorials are on the NFXLIB site

 [NFXLIB - Documentation/Guides/Tutorials](http://nfxlib.com)
 
 

 
## NUGET
*UNDER CONSTRUCTION*
As of 2018-01-14 you can use NFXv5 only if you get it here and build it. We are still working on publishing Nuget packages


[NFX Packages TO BE RELEASED for NFXv5 ](https://www.nuget.org/profiles/itadapter)

Older (NFXv3) Package home:

cmd | Description
 -------|------
 `pm> install-package NFX` | NFX Core Package (App, Pile, Glue, Log, Instr etc.)
 `pm> install-package NFX.Web`| NFX Web (Amazon S3, Google Drive, SVN, Stripe, Braintree, PPal, FB, Twtr etc.) 
 `pm> install-package NFX.Wave`| NFX Wave Server + MVC 
 `pm> install-package NFX.MsSQL`| NFX Microsoft SQL Server Provider 
 `pm> install-package NFX.MySQL`| NFX MySQL RDBMS Provider (CRUD etc.)
 `pm> install-package NFX.MongoDB`| NFX MongoDB Proivder (CRUD etc.) + Native Client 
 `pm> install-package NFX.WinForms`| NFX WinForms (for legacy)
 `pm> install-package NFX.Azure`| NFX Azure IaaS Provider WIP/pre-release
 `pm> install-package NFX.Erlang`| NFX Erlang Language + OTP Node


## Resources

### Big Memory Object Pile + Cache

[NFX/ApplicationModel/Pile](./src/NFX/ApplicationModel/Pile)

### Various Demo Projects

 [https://github.com/aumcode/nfx-demos](https://github.com/aumcode/nfx-demos)
 
 [ https://github.com/aumcode/howto()](https://github.com/aumcode/howto)
 
 
## NFX Provides

* Unified App Container
  - Unified app models: console, web, service, all have: user, session, security, volatile state
  - Configuration: file, memory, db, vars, macros, structural merges, overrides, scripting
  - Dep injection: inject dependencies
  - Logger: async file, debug, db destinations with graphs, SLA rules, filtering and routing
  - Security: declr and/or imperative permission model, strong password manager, virtual credentials 
  
* [**Big Memory Pile** ](./src/NFX/ApplicationModel/Pile)
  - Pile memory manager: keeps hundreds of millions of CLR objects in memory without GC pauses
  - Distributed Pile (objects stored on cluster nodes)
  - Pile Cache: materialize 2,000,000 CLR objects/sec in-memory on a 4 core machine
  
* **Full Web Stack**
  - Web server
  - Rule-based network Gate (business firewall)
  - MVC: filters, attributes, complex model binding, security, web API, MVVM binding
  - Template
  - Client JS lib + MVVM
  
* Hybrid data access layer
  - RDBMS (we use: MySQL, MsSQL)
  - NoSQL (we use: MongoDB, Elastic search, Erlang OTP)
  - Native ultra-fast MongoDB driver (socket based achieving 200K+reads/sec)
  
* Full Instrumentation Suite
  - Gauges and Events keyed on business enities
  - Instrumentation buffers
  - Injectable instrumentation sinks (i.e. telemetry receiver)
  - Cluster real-time map:reduce on zones and regions
    
* High-level service oriented stack "Glue"
  - Contract based design with security
  - Injectable bindings: i.e. "Async TCP"
  - 150,000 ops/sec two-way calls using *business objects* (not byte array) on a 4 core machine
  - Unistack payload teleportation (no need to decorate various classes, teleport as-is)
  
* Serialization Suite
  - Slim: the *fastest general purpose* CLR serializer, very well tested and proven processing 
  - Teleportation: moving *CLR objects as-is without any extra metadata* between processes
  - BSON: an efficient BSON implementation
  - JSON: includes multi-language selective serialization of large graphs
  
* Erlang Support: including types, serialization and full OTP node
* Virtual File Systems: Amazon S3, SVN, Local, (Google Drive and DropBox created by others)
* Virtual payment processing: Braintree, Stripe, PayPal
* Code Analysis (building lexers/parsers)
* Type Conversion Accessors (i.e. object.AsInt(dflt)....)


**IMPORTANT!**

NFX uses the very Base-Class-Lib of .NET:
* Basic/primitive types: string, ints, doubles, decimal, dates, +Math
* Parallel task library: 25% of features - create, run, wait for completion,
  Task, Parallel.For/Each
* Collections: List, Dictionary, ConcurrentDictionary, HashSet, Queue
* Threading: Thread, lock()/Monitor, Interlocked*, AutoresetEvent
* Various: Stopwatch, Console, WinForms is used for SOME interactive tests(not needed for operation)
* Some ADO references (Reader/SQLStatement) in segregated data-access components
* Reflection API
* Drawing 2D (Graphics)

**NFX does not use any 3rd party components.** *(with the exception of system packages which are not in the CLR/sys "core" yet, such as System.Buffers, ValueTask(T) etc...)*

