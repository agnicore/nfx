# SLIM

**NFX** Slim is the proprietary compressing binary format serializer. 
It does not support versioning (as it is not needed for problems that Slim solves). 
It is the most important piece of technology as Glue and Big Memory depend on it. 

Slim serializer uses dynamically-created/compiled/cached expression trees for the purpose of IL generation. 
The expressions are created for every new type that Slim encounters. 

Unlike Protobuff (and many others) Slim is the only fully-compatible CLR serializer as it supports true type descriptions/polymorphism, `ISerializable`, `IDeserializationCallback`, `OnSerialize/OnDeserialize` family methods.

Slim is very close to Protobuff in both size and speed and is the fastest full CLR binary serializer on the market Z(the only other full-CLR supporting serializer is MS BinaryFormatter which is orders of magnitude slower than Slim).

We call transparent CLR object serialization - “teleportation”. 
It is an act of moving any CLR object instance as long as it does not have unmanaged handles (or handles it using ISerializable). 
This process can not be compared to Thrift or Protobuf. 
It is very different from “zero-copy” serializers as the later are just a trick - they require you to pre-serialize data by using their APIs, whereas Slim works with native CLR objects (no APIs needed).

Slim also poses capability of injectable wire-format layer that allows for custom/native/by-hand serialization of custom types, for example NLSMap. 
This sometimes gives 2x-6x speed boost. 

By default, slim uses variable bit encoding for integer types and sparse arrays.