# Glue

`NFX.Glue` technology allows to develop distributed, service-oriented applications in a simple and efficient manner with minimal effort. 
Glue provides opportunity to link  ("glue") different nodes in the distributed system (i.e. Agni nodes). 
It is, in some regard, an  analogue of Microsoft WCF but much easier to use.

The concept of Glue is very similar to what a concept of WCF is in terms of message passing, 
it is the message oriented framework where you exchange messages between contract-implementing/consuming end points.

For data exchange Glue uses CLR object teleportation via Slim Serializer that does not require to assign any auxiliary attributes to data 
(like data-member, data-contract and etc) - you can transfer any CLR object (as long as it does not have unmanaged state). 


## Contents

* [Overview](./overview.md)

* [Bindings](./bindings.md)

* [Security](./security.md)