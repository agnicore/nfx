# BSON

**NFX** provides support for standard BSON format. 
It is used in MongoDB driver. 
Unlike the official MongoDB implementation, the one in **NFX** is more efficient as it creates less class instances in ram, thus relieving unnecessary GC pressure. 
Instead of holding Element and Value instances, **NFX** holds typed element instance i.e. `BSONIntegerElement` - this reduces the number of objects by a factor of 2.