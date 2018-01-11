# Stateful Server

Stateful server stores its state during each client session.
Below is step-by-step guide to create **NFX.Glue** stateful server application.
The guide is quite brief in some places. For details, please check the [demo](./echo.md).

1\. Create new blank C# solution and add Class Library project `DemoContracts` 
with the reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>)

2\. Add IStatefulService.cs file with the following contents:

```cs
[Glued]      
[LifeCycle(ServerInstanceMode.Stateful, TimeoutMs = 1000000)]
public interface IStatefulService
{
  [Constructor]
  void Init();
  
  void Add(int value);
  
  int GetValue();
  
  [Destructor]
  void Done();
}
```

Notice `Glued` and `LifeCycle` attributes. The former marks all classes that implement the contract as a **NFX.Glue** services,
while the latter sets up stateful lifetime mode for the service. Also notice methods marked as `Constructor` and `Destructor`. 
It may be neccessary to have some "setup" and "teardown" logic for stateful service.
Client should call method decorated with `Constructor` attribute before any other.

3\. Add new C# Console Application project `DemoServer` with the references to **NFX** and `DemoContracts` project. Add DemoServer.laconf laconic configuration file:
```js
application
{
  object-store
  {
    name="Main Object Store"
    guid="76F44831-E92A-42BB-9294-E806300405C9"
    object-life-span-ms="180000"
    bucket-count="1024"

    provider
    {
      name="Disk Object Store Provider"
      type="NFX.ApplicationModel.Volatile.NOPObjectStoreProvider"
    }
  }

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
        contract-servers=$"DemoServer.StatefulService, DemoServer"
      }
    }
  }
}
```
Be sure to set Build Action : None and Copy to Output Directory: Always in the config file properties.

Notice `object-store` configuration section. It is neccessary for stateful services to have this section in configuration file.


4\. Add StatefulService.cs file with the contents:

```cs
[ThreadSafe]
public class StatefulService : IStatefulService
{
  private int m_State;
     
  public void Init()
  {
  }
  
  public void Add(int value)
  {
    // this is our thread-safe addition
    Interlocked.Add(ref m_State, value);
  } 
  
  public int GetValue()
  {
    return m_State;
  }
  
  public void Done()
  {
  }
}
```

Notice the use of `ThreadSafeAttribute` which tells the Glue runtime that this class implements thread-safety internally, 
so Glue doesn't need to lock its instance.

5\. Add the following code to the `Main` method of DemoServer.Program.cs file

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

6\.  Add the last one C# Console Application project `DemoClient` with the references to **NFX** and `DemoContracts` project.
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

7\. Create client proxy class that will operate with the service above:

gluec "$(SolutionDir)DemoContracts/bin/Debug/DemoContracts.dll" /o out="$(SolutionDir)DemoClient" cl-suffix="AutoClient"

Rebuild `DemoContracts` project and add the file StatefulServiceAutoClient.cs to the `DemoClient` project (the file will appear in the root of the project).

8\. Add the following code to the `Main` method of DemoClient.Program.cs:

```cs
using (var application = new ServiceBaseApplication(args, null))
{
  using (var client = new StatefulServiceAutoClient("sync://localhost:8080"))
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
private static void exeCommand(StatefulServiceAutoClient client, string[] cmd)
{
  if (cmd[0].Equals("init"))
  {
    client.Init();
    return;
  }

  if (cmd[0].Equals("add"))
  {
    var num = cmd[1].AsInt();
    client.Add(num);
    return;
  }
  
  if (cmd[0].Equals("result"))
  {
    var value = client.GetValue();
    Console.WriteLine("Server state: "+value);
    return;
  }
  
  if (cmd[0].Equals("done"))
  {
    client.Done();
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

15\. Run `DemoServer` server host and start the `DemoClient` client application. 
Execute the following commands sequentially: "init", "add 1", "add 3", "result", "done". The output will be 4.

Execution of any command after "done" will cause an exception.
