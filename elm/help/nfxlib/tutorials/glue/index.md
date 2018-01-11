# Glue

NFX.Glue technology is a high-level service oriented stack that allows to develop distributed, service-oriented applications 
in a simple and efficient manner with minimal effort.
NFX.Glue provides

* Contract based design with security

* Injectable bindings: i.e. "Async TCP"

* 150,000 ops/sec two-way calls using business objects (not byte array) on a 4 core machine

* Unistack payload teleportation (no need to decorate various classes, teleport as-is)

<br>

Any **NFX.Glue** solution naturally includes at least three projects:
* Contracts library - contains contracts shared between server- and client-side such as service contracts and data contracts
* Server host - contains server-side implementation of service contracts and also provides host process for them
* Client app - contains client-side code

Below one can find some step-by-step examples demonstrate basic **NFX.Glue** features.

Full demo projects can be found by <a href="https://github.com/aumcode/nfx-demos" target="_target">link</a>.

## Contents

* [Echo Test](./echo.md)

* [Stateful Server](./stateful.md)

* [Data Contracts](./data-contract.md)

* [High Load Server](./high-load.md)

* [Security](./security.md)