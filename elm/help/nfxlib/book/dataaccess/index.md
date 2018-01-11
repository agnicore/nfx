# Data Access

**NFX** Data Access was designed to be very flexible and support single and non-homogeneous data stores. 
Contemporary data stores can not be modeled after client/server(in RDBMS sense) RDBMS/SQL-only concepts, 
instead modern data is usually distributed and arrives from different sources/providers. 

For example, a high-frequency-trading application data store may have a read-only market feed that is provided by subscription channels (async socket push) 
and a typical "back office" system where trading and strategy definitions are kept. 
In this instance, the subscription feeds take some "query" (i.e. a symbol and sampling rate "MSFT^"/1minute) 
while "backoffice" query may be more of a kin to "select all strategies where risk-category < ‘75%’".

## Contents

* [Overview](./overview.md)

* [Data Objects, Schema and Metadata](./objects.md)

* [Querying and CRUD](./querying.md)

* [Providers](./providers.md)

* [Relational Schema](./rschema.md)