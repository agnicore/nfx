# REST Server

With **NFX.WAVE** you can create Web API in a REST manner.
In the present tutorial we demonstrate how this can be done.
Some moments are described quite briefly, so please see previous tutorials for details.

Let's create the simplest **NFX.Wave** REST API for the Clinic App - a web application that stores
information about patients of some clinic.

1\. Create new C# Console Application project `ClinicApp`.

2\. Add reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>).

3\. Go to `Program.cs` and create `WaveServer` instance in the scope of **NFX** `ServiceBaseApplication` application container:

```cs
using System;
using NFX.ApplicationModel;
using NFX.Wave;

namespace ClinicApp
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        using (var app = new ServiceBaseApplication(args, null))
        using (var server = new WaveServer())
        {
          server.Configure(null);
          server.Start();
          Console.WriteLine("server started...");
          Console.ReadLine();
        }
      }
      catch (Exception error)
      {
        Console.WriteLine("Critical error:");
        Console.WriteLine(error);
        Environment.ExitCode = -1;
      }
    }
  }
}
```

4\. Create the following project folders: `Controllers`, `Data`, `Data\Models`. 

5\.
Let us add application data infrastructure.

Add the following classes in `Data\Models` folder:

```cs
public class Doctor : TypedRow
{
  [Field(required: true)]
  public int ID { get; set; }

  [Field(required: true, maxLength: 32, description: "First Name")]
  public string FirstName { get; set; }

  [Field(required: true, maxLength: 32, description: "Last Name")]
  public string LastName { get; set; }

  public string FullName { get { return "{0} {1}".Args(FirstName, LastName); } }
}

public class Patient : TypedRow
{
  [Field(required: true)]
  public int ID { get; set; }

  [Field(description: "Doctor")]
  public int? DoctorID { get; set; }

  [Field(required: true, maxLength: 32, description: "First Name")]
  public string FirstName { get; set; }

  [Field(required: true, maxLength: 32, description: "Last Name")]
  public string LastName { get; set; }

  [Field(required: true, min: 0, description: "Age")]
  public int Age { get; set; }

  public void UpdateFrom(Patient other)
  {
    this.DoctorID  = other.DoctorID;
    this.FirstName = other.FirstName;
    this.LastName  = other.LastName;
    this.Age       = other.Age;
  }
}
```

Note that both models are inherited from `NFX.DataAccess.CRUD.TypedRow` class.

6\. We also need some data store to persist our data. In real life application some RDBMS most likely will be used. 
In our toy example the data will be stored in memory for the case of simplicity.

Let us implement our data store. Add the following `ClinicDataStore` class to `Data\` folder

```cs
public class ClinicDataStore : ApplicationComponent, IDataStoreImplementation, IConfigurable
{
  private readonly Dictionary<int, Doctor>  m_Doctors  = new Dictionary<int, Doctor>();
  private readonly Dictionary<int, Patient> m_Patients = new Dictionary<int, Patient>();

  public ClinicDataStore()
  {
    m_Doctors.Add(1, new Doctor { ID=1, FirstName="Stephan", LastName="Bankh" });
    m_Doctors.Add(2, new Doctor { ID=2, FirstName="Stan",    LastName="Ulem" });

    m_Patients.Add(1, new Patient { ID=1, FirstName="Johny",  LastName="Oldman",      Age=34, DoctorID=1 });
    m_Patients.Add(2, new Patient { ID=2, FirstName="Rob",    LastName="Closeheimer", Age=43, DoctorID=1 });
    m_Patients.Add(3, new Patient { ID=3, FirstName="Andrew", LastName="Sugarov",     Age=29, DoctorID=2 });
  }

  #region CRUD
  
  #endregion

  #region IDataStoreImplementation, IApplicationComponent, IConfigurable

  #endregion
}
```

We use the concept of **NFX** Data Store - a single application data access point.
After proper setup of this class in configuration file, its instance will be accessible through `App.DataStore` static property.

Let us leave interfaces implementation default

```cs

#region IDataStoreImplementation, IApplicationComponent, IConfigurable

  public string TargetName { get { return "ClinicDB"; } }
  public StoreLogLevel LogLevel { get; set;}
  public bool InstrumentationEnabled { get; set; }
  public IEnumerable<KeyValuePair<string, Type>> ExternalParameters { get { return null; } } 
  
  public void TestConnection()
  {
  }
  
  public void Configure(IConfigSectionNode node)
  {
    ConfigAttribute.Apply(this, node);
  }
  
  public IEnumerable<KeyValuePair<string, Type>> ExternalParametersForGroups(params string[] groups)
  {
    throw new NotImplementedException();
  }
  
  public bool ExternalGetParameter(string name, out object value, params string[] groups)
  {
    throw new NotImplementedException();
  }
  
  public bool ExternalSetParameter(string name, object value, params string[] groups)
  {
    throw new NotImplementedException();
  }

#endregion
```

Add basic CRUD logic to our data store:

```cs
#region CRUD

  public void CreatePatient(Patient patient)
  {
    var id = m_Patients.Keys.Max() + 1;
    patient.ID = id;
    m_Patients.Add(id, patient);
  }
  
  public IEnumerable<Patient> GetPatients()
  {
    return m_Patients.Values;
  }
  
  public Patient GetPatient(int id)
  {
    return m_Patients[id];
  }
  
  public void UpdatePatient(Patient patient)
  {
    var persisted = m_Patients[patient.ID];
    persisted.UpdateFrom(patient);
  }
  
  public void DeletePatient(int id)
  {
    m_Patients.Remove(id);
  }
  
  public Doctor GetDoctor(int id)
  {
    return m_Doctors[id];
  }
  
  public IEnumerable<Doctor> GetDoctors()
  {
    return m_Doctors.Values;
  }

#endregion
```

7\. Now we need to add API controller to our project. 

Add the class `Patients` to `Controllers` folder:

```cs
using NFX;
using NFX.Serialization.JSON;
using NFX.Wave.MVC;
using ClinicApp.Data;
using ClinicApp.Data.Models;

namespace ClinicApp.Contollers
{
  public class Patients : Controller
  {
    [Action("index", 0, "match{methods=GET accept-json=true}")]
    public object Index()
    {
      var data = (ClinicDataStore)App.DataStore;
      var patients = data.GetPatients().OrderBy(p => p.ID);
      var result = new JSONDataArray(patients);

      return result;
    }

    [Action("details", 0, "match{methods=GET accept-json=true}")]
    public object Details_GET(int id)
    {
      var data = (ClinicDataStore)App.DataStore;
      var patient = (id==0) ? new Patient() : data.GetPatient(id);
      return patient;
    }

    [Action("details", 1, "match{methods=POST accept-json=true}")]
    public object Details_POST(Patient patient)
    {
      var data = (ClinicDataStore)App.DataStore;
      data.CreatePatient(patient);

      return NFX.Wave.SysConsts.JSON_RESULT_OK;
    }

    [Action("details", 2, "match{methods=PUT accept-json=true}")]
    public object Details_PUT(Patient patient)
    {
      var data = (ClinicDataStore)App.DataStore;
      data.UpdatePatient(patient);

      return NFX.Wave.SysConsts.JSON_RESULT_OK;
    }

    [Action("details", 3, "match{methods=DELETE accept-json=true}")]
    public object Details_DELETE(int id)
    {
      var data = (ClinicDataStore)App.DataStore;
      data.DeletePatient(id);
      return NFX.Wave.SysConsts.JSON_RESULT_OK;
    }
  }
}
```

8\. The last step is to add config file with server settings, MVC handler path matches etc.
```js
application
{
  data-store
  {
    type="ClinicApp.Data.ClinicDataStore, ClinicApp"
  }

  wave
  {
    server
    {
      prefix { name="http://localhost:8070/" }
      dispatcher
      {
        handler
        {
          order=1
          name="Stock Content Embedded Site"
          order=1000
          type="NFX.Wave.Handlers.StockContentSiteHandler, NFX.Wave"
          match{ path="/stock/{*path}"}
        }

        handler
        {
          order=10
          type="NFX.Wave.Handlers.MVCHandler, NFX.Wave"
          type-location
          {
            assembly="ClinicApp.exe"
            ns { name="ClinicApp.Contollers" }
          }

          match
          {
            order=1
            path="patients"
            var{ name=type default='Patients' }
            var{ name=mvc-action default='Index' }
          }

          match
          {
            order=10
            path='patients/{id}'
            var{ query-name=* }
            var{ name=type default='Patients' }
            var{ name=mvc-action default='Details' }
          }

          match
          {
            order=100
            path="/{type=Patients}/{mvc-action=Index}"
          }
        }
      }
    }
  }
}
```
Make sure that Build Action is set to None and Copy to Output Directory is set to Copy Always.

9\. Our REST API is ready to use. You can test it via any HTTP request generator. The following **curl** request

```
curl -H "Accept: application/json"
     -X GET http://localhost:8070/patients/
```

will return

```
[
  {"ID":1,"DoctorID":1,"FirstName":"Johny","LastName":"Oldman","Age":24},
  {"ID":2,"DoctorID":1,"FirstName":"Rob","LastName":"Closeheimer","Age":43},
  {"ID":3,"DoctorID":2,"FirstName":"Andrew","LastName":"Sugarov","Age":29}
]
```

Try requests

```
curl -i
     -H "Accept: application/json" -H "Content-Type: application/json" 
     -X POST -d {\"FirstName\":\"Bill\",\"LastName\":\"Zimmer\",\"Age\":55} http://localhost:8070/patients/0
```
     
```
curl -i
     -H "Accept: application/json" -H "Content-Type: application/json" 
     -X PUT -d {\"ID\":1,\"FirstName\":\"New\",\"LastName\":\"Name\",\"Age\":55} http://localhost:8070/patients/1
```

```
curl -i 
     -H "Accept: application/json" 
     -X DELETE http://localhost:8070/patients/3
```
     
followed by GET request to see our API in work.