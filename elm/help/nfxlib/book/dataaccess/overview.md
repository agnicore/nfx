# Overview

**NFX** Data Access starts from the root Data: IDataStore property on the root Application container interface. 
It is just that - a marker interface. Actual application shall derive from it and implement their own data stores to fit their particular purpose. 

**NFX** provides two primary data store "schemes":

* CRUD data store - usually implementors of ICRUDDataStore - provide automatic generation of database (not necessarily SQL-only) commands to service CRUD (insert/update/delete/upsert) requests

* Composite data stores - these are more "layered" data stores that use composition of CRUD and other data stores. They act as a business facades to your data

Simple business applications may use CRUD data stores directly as they suffice in 90% of cases, however most realistic business pas should create a business-oriented facades - composite data stores - that better represent logic sections of the problem domain. 

**Example:**
```csharp
var shippers = MyECommerceApp.Data.ShippingLogic.GetShippersForRegion("USA", "OH");
```

Notice the use of the typed-application facade "MyECommerceApp". 
It is usually a static class that provides a shortcut access to a TYPED data store per your particular app:

```csharp
public IMyECommerceDataStore Data 
{
  get { return App.Data as IMyECommerceDataStore; } // or throw 
}
```