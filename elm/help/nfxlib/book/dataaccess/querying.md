# Querying and CRUD

**NFX** data access does not use LINQ on purpose. 
This is because it is impossible to efficiently map LINQ to a possibly non-homogeneous data store that joins/takes data from different sources. 
Instead, data access in **NFX** is based on virtual queries. Queries get resolved by CRUDDataStore into `ICRUDQueryHandler`.  

When data must be loaded, instead of using ORM/and/or LINQto*(entity) a developer would execute a query which has a name and named parameters. 
The query DOES NOT specify the text of the actual backend query as it is not necessarily a scriptable backend which is being queried. 
A query specifies the NAME of command. It is a "command object" pattern per GOF jargon.

```csharp
var qryPatients = new Query<PatientRow>("GetPatientsByProvider")
{
  new Query.Param("HospitalID", hID),
  new Query.Param("ProviderID", myProviderID)
};

var patients = MyApp.Data.LoadOneRowset(qryPatients);

foreach(var patient in patients)
{
  // patient is of type PatientRow
} 
```

The "GetPatientsByProvider" name will get resolved into either of the two the ICRUDQueryHandler implementations:
* An embedded textual script `ICRUDScriptQueryHandler`
* A custom class that implements `ICRUDQueryHandler `

If a query can be executed using SQL or JSON (in case of MongoDB) or ErlTuple(for Erlang Mnesia or RPC).

Scripts can have target suffixes, as the data access layer supports multi targeting, so it is possible to target say ORACLE and MSSQL (and others) by the same codeset. 
The difference would be in the file suffixes.

An Important capability is the class `CRUDQueryHandlers` - they allow for arbitrary/custom implementation in C#, 
so in case some target does not support some function (i.e. MySQL does not support CONNECT BY), 
we can implement this (just)for MySQL using a few query requests and C# code, while ORACLE implementation remains script-only. 
This approach allows for an infinite flexibility.