# Application container

Although not required, any typical application in **NFX** executes in the application container. 
It is just a global class that provides root services such as:

* **Component Model** - keeps track of process components (i.e. logger, business logic etc..)

* **Configuration** - provides root configuration for the instance

* **Logging** - sync/async log queue with injectable destinations/sinks (file, db etc…)

* **Instrumentation** - named gauges and events, real-time source key reduce and telemetry

* **Inter-process/node communication** - contract-based programming akin to WCF

* **DataStore** - access data (i.e. a notepad app may use FileDataStore to store files on disk)

* **TimeSource** - injectable time/zone source

* **EventTimers** - schedule with sync/async event handlers, injectable from config

* **SecurityManager** - handles roles/users/permissions resolution (has nothing to do with MS LDAP but may be used on that as one of the possibilities)

* **Volatile ObjectStore** - check in/out pattern for objects that may need to survive process recycle

* **Session Context** - session a kin to web session - in any app (i.e. console session)

* **Dependency Injection/Starters** - injects components from config. Starters - named components that start along with the app

## Contents

* [Setup Entry Point](./appsetup.md)