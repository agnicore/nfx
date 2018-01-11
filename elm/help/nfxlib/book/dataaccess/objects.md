# Data Objects, Schema and Metadata

**NFX** data access concept revolves around CRUD primitives: Rows and Rowsets/Tables.

A Row represents a row of data, not necessarily relational, that has fields. 
A row may have a hierarchical (nested) structure. A row may represent a row from RDBMS, file, document (i.e. MongoDB) and other sources. 
Some rows are read only, and only used to project data into (i.e. from SQL), whereas majority of rows is usually "writable" akin to an "Active Record" pattern, 
they can be saved back into the data store.

Every row has a Schema - a list of field definitions with metadata attributes.
There are two primary types of rows, both inherit from Row: DynamicRow and TypedRow.

Dynamic rows store data in `object[]` and the data is "shaped" according to the Schema.
TypedRows are "shaped" by their declared properties decorated with [FIELD] attribute.

```csharp
public class Person : TypedRow
{
  [Field(backendName: "pk", key: true, required: true)]
  public string ID{ get; set;}
  
  [Field]
  public string Name{ get; set;}
  
  [Field(required: true)]
  public DateTime? DOB{ get; set;}
}
```

Rows have `Validate(string target)` method that returns either null or in case of validation violations an instance of `ValidationException`. 
The exception is not thrown, it is returned instead, this is done for speed (throw is 20-100 slower than return) as validation exceptions are pretty much a norm.

As POCO classes, rows are teleportable with Slim, so they can be transparently stored in Pile/Cache and be teleported to other machines/processes using Glue.

Rows can be used in `List<>`, however NFX provides a more handy structure: Rowsets and Tables. 
The difference stems from the recognition of the primary key by the Table - it is a form of a sorted -by-pk rowset (with binary search findkey). 
Tables are good for data sync in distributed systems (i.e. used in trading system fed from Erlang OTP).