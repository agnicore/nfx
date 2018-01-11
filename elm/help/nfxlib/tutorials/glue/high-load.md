# High Load Server

One of the main benefit of **Glue** technology is speed, which is achieved by fast and effectiove **Slim** technology.
Below is step-by-step guide to of high-load server. You can also choose between parallel and syncronous version of the test.
The guide is quite brief in some places. For details, please check the [demo](./echo.md).

1\. Create new blank C# solution and add Class Library project `DemoContracts`
with the reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>)

2\. Add IHighLoadService.cs file with the following contents:

```cs
[Glued]
public interface IHighLoadService
{
  /// <summary>
  /// Does not generate call receipt (one-way call)
  /// </summary>
  [OneWay]
  void Ping1();
  
  /// <summary>
  /// Generates call receipt (two-way call)
  /// </summary>
  void Ping2();
}
```

3\. Add new C# Console Application project `DemoServer` with the references to **NFX** and `DemoContracts` project. Add DemoServer.laconf laconic configuration file:
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
        contract-servers=$"DemoServer.HighLoadService, DemoServer"
      }
    }
  }
}
```
Be sure to set Build Action : None and Copy to Output Directory: Always in the config file properties.

4\. Add HighLoadService.cs file with the contents:

```cs
public class HighLoadService : IHighLoadService
{
  public void Ping1()
  {
  }
  
  public void Ping2()
  {
  }
}
```

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

6\. Add the last one C# Console Application project `DemoClient` with the references to **NFX** and `DemoContracts` project.
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

Rebuild `DemoContracts` project and add the file HighLoadServiceAutoClient.cs to the `DemoClient` project (the file will appear in the root of the project).

8\. Add the following code to the `Main` method of DemoClient.Program.cs:

```cs
using (var application = new ServiceBaseApplication(args, null))
{
  using (var client = new HighLoadServiceAutoClient("sync://localhost:8080"))
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
private static void exeCommand(HighLoadServiceAutoClient client, string[] cmd)
{
  var ops = cmd[0].AsInt();
  var isOneWay = cmd[1].AsBool();
  var isParallel = cmd[2].AsBool();

  var timer = new Stopwatch();
  timer.Start();

  if (isOneWay)
  {
    if (isParallel)
      Parallel.For(0, ops, i => client.Ping1());
    else
      for (int i=0; i<ops; i++)
        client.Ping1();
  }
  else
  {
    if (isParallel)
      Parallel.For(0, ops, i => client.Ping2());
    else
      for (int i = 0; i < ops; i++)
        client.Ping2();
  }

  timer.Stop();

  Console.WriteLine("Executed:    {0} ops {1} {2}", ops, isOneWay ? "one-way" : "", isParallel ? "parallel" : "");
  Console.WriteLine("Total time:  {0} ms", timer.Elapsed.TotalMilliseconds);
  Console.WriteLine("Performance: {0} ops/sec", (ops / timer.Elapsed.TotalSeconds).AsInt());
}
```

You will also need to add usings

```cs
using System.Diagnostics;
using NFX;
using NFX.ApplicationModel;
using DemoContracts.GluedClients;
```

9\. Run `DemoServer` server host and start the `DemoClient` client application.
Execute the following commands sequentially: "100000 true false", "100000 true true", "100000 false true", "100000 false false". 
The output will be like

```
Executed:    100000 ops one-way
Total time:  1005 ms
Performance: 99414 ops/sec

Executed:    100000 ops one-way parallel
Total time:  433 ms
Performance: 230915 ops/sec

Executed:    100000 ops parallel
Total time:  1028 ms
Performance: 97246 ops/sec

Executed:    100000 ops
Total time:  4008 ms
Performance: 24949 ops/sec
```