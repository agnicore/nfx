# Data Contracts

The concept of **Glue** technology is very similar to other service-oriented frameworks in terms of data passing. 
**Glue** is the message oriented framework where you exchange messages between contract-implementing/consuming endpoints.
The types that you can pass from client to server and back are not limited to elementary .NET types. Any custom type can be transferred between client and server
via [**Slim**](../../book/serialization/slim.md) serialization.

Below is step-by-step guide to create **NFX.Glue** test server application that demonstrates non-elementary types transfer.
The guide is quite brief in some places. For details, please check the [demo](./echo.md).

1\. Create new blank C# solution and add Class Library project `DemoContracts` 
with the reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>)

2\. Add the following types:

```cs
public enum Country
{
 Unknown,
 Argentina,
 England,
 Russia,
 USA
}

public class Location
{
  public Country Country { get; set; }
  public string City { get; set; }
}

public class Person
{
  public string Name { get; set; }
  public DateTime DOB { get; set; }
  public Country Citizenship { get; set; }
  public List<Location> Locations { get; set; }
}
```

3\. Add IPersonService.cs file with the following contents:

```cs
[Glued]
[LifeCycle(ServerInstanceMode.Singleton)]
public interface IPersonService
{
  void Set(Person person);

  Person FindByName(string pattern); 
}
```

Notice the service is declared as singleton.

4\. Add new C# Console Application project `DemoServer` with the references to **NFX** and `DemoContracts` project. Add DemoServer.laconf laconic configuration file:
```js
application
{
  glue
  {
    bindings
    {
      binding
      {
        name="sync"
        type="NFX.Glue.Native.SyncBinding, NFX"
      }
    }
  
    servers
    {
      server
      {
        node="sync://localhost:8080"
        contract-servers=$"DemoServer.PersonService, DemoServer"
      }
    }
  }
}
```
Be sure to set Build Action : None and Copy to Output Directory: Always in the config file properties.

5\. Add PersonService.cs file with the contents:

```cs
public class PersonService : IPersonService
{
  private List<Person> m_Persons;
  
  public PersonService()
  {
    m_Persons = new List<Person>();
  }
  
  public Person FindByName(string name)
  {
    lock(m_Persons)
      return m_Persons.Where(p => p.Name.Equals(name)).FirstOrDefault();
  }
  
  public void Set(Person person)
  {
    if (person == null) return;
    lock(m_Persons)
      m_Persons.Add(person);
  }
}
```

This class is singleton by contract and it maintains global state: `m_Persons`.

6\. Add the following code to the `Main` method of DemoServer.Program.cs file

```cs
using (var application = new ServiceBaseApplication(args, null))
{
  Console.WriteLine("server is running...");
  Console.WriteLine("Glue servers:");
  foreach (var service in App.Glue.Servers)
    Console.WriteLine("   " + service);
  
  Console.ReadLine();
}
```

7\. Add the last one C# Console Application project `DemoClient` with the references to **NFX** and `DemoContracts` project.
Add DemoClient.laconf laconic configuration file:

```js
application
{
  glue
  {
    bindings
    {
      binding
      {
        name="sync"
        type="NFX.Glue.Native.SyncBinding, NFX"
      }
    }
  }
}
```

Again, be sure to set Build Action : None and Copy to Output Directory: Always in the file properties.

8\. Create client proxy class that will operate with the service above:

gluec "$(SolutionDir)DemoContracts/bin/Debug/DemoContracts.dll" /o out="$(SolutionDir)DemoClient" cl-suffix="AutoClient"

Rebuild `DemoContracts` project and add the file PersonServiceAutoClient.cs to the `DemoClient` project (the file will appear in the root of the project).

9\. Add the following code to the `Main` method of DemoClient.Program.cs:

```cs
using (var application = new ServiceBaseApplication(args, null))
{
  using (var client = new PersonServiceAutoClient("sync://localhost:8080"))
  {
    while (true)
    {
      var input = Console.ReadLine();
      var cmd = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      exeCommand(client, cmd);
    }
  }
}
```

Add the following method:

```cs
private static void exeCommand(PersonServiceAutoClient client, string[] cmd)
{
  if (cmd[0].Equals("add"))
  {
    var person = new Person
    {
      Name = cmd[1],
      DOB = DateTime.Now.AddYears(-cmd[2].AsInt()),
      Citizenship = Country.Argentina,
      Locations = new List<Location> { new Location { Country = Country.USA, City = "NY" } }
    };

    client.Set(person);
    return;
  }

  if (cmd[0].Equals("find"))
  {
    var name = cmd[1];
    var result = client.FindByName(name);
    if (result == null)
      Console.WriteLine("Not found");
    else
      Console.WriteLine("Name: {0}, Age: {1}, Citizenship: {2}, Locations: {3}",
                        result.Name,
                        (int)(DateTime.Now - result.DOB).TotalDays / 366,
                        result.Citizenship,
                        result.Locations.Count());

    return;
  }
}
```

You will also need to add usings

```cs
using NFX;
using NFX.ApplicationModel;
using DemoContracts.GluedClients;
```

10\. Run `DemoServer` server host and start the `DemoClient` client application. 
Execute the following commands sequentially: "add John 12", "add Anna 23", "find John", "find Ivan". The output will be

```
Name: John, Age: 20, Citizenship: Argentina, Locations: 1
Name: Anna, Age: 23, Citizenship: Argentina, Locations: 1
Not found
```