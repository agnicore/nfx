# Security

**NFX** proposes its own security model appropriate for any application type.
Below is the demonstration of **NFX** Security model for the case of **NFX.Glue** client-server application.
It is possible add users with certain credentials and permissions in security/users section  of *.laconf configuration file, 
and then seal any server methods or the whole server by applying `PermissionAttribute` attribute which guaranted that only users with appropriate permissions can
access the methods.

The guide is quite brief in some places. For details, please check the [demo](./echo.md).

1\. Create new blank C# solution and add Class Library project `DemoContracts`
with the reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>)

2\. Add ISecureService.cs file with the following contents:

```cs
/// <summary>
/// Only those users who have superman permission can call the service
/// </summary>
[Glued]
[AuthenticationSupport]
[AdHocPermission("specialized", "superman", AccessLevel.VIEW)]
public interface ISecureService
{
  string Echo(string message);
  
  /// <summary>
  /// This method can only accessed by superman who is also a president
  /// </summary>
  [AdHocPermission("specialized", "president", AccessLevel.VIEW)]
  string PresidentEcho(string message);
}
```

You will also need to add following usings

```
using NFX.Glue;
using NFX.Security;
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
        contract-servers=$"DemoServer.SecureService, DemoServer"
      }
    }
  }
  
  security
  {
    users
    {
      user
      {
        name="Clinton"
        description="Bill Clinton"
        id="clinton"
        password= "{ hash: 'fyQ5RvWb2LxW90s+fBKKJg==', salt: 'salt', algo: 'MD5' }" // phash newyorksalt
        rights
        {
          specialized { superman {level=1} president {level=1} }
          gov { executive {level=0} } // he is not president
        }
      }
      
      user
      {
        name="Trump"
        description="Donald Trump"
        id="trump"
        password= "{ hash: 'fyQ5RvWb2LxW90s+fBKKJg==', salt: 'salt', algo: 'MD5' }" // phash chicagosalt
        rights
        {
          specialized { superman {level=1} president {level=1} }
          gov { executive {level=1} } // he is current president
        }
      }
    }
  }
}
```
Be sure to set Build Action : None and Copy to Output Directory: Always in the config file properties.

4\. Add SecureService.cs file with the contents:

```cs
public class SecureService : ISecureService
{
  public string Echo(string message)
  {
    return "Secure service echo: " + message;
  }
  
  [AdHocPermission("gov", "executive", AccessLevel.VIEW)]
  public string PresidentEcho(string message)
  {
    return "President secure echo: " + message;
  }
}
```

You will also need to add following `NFX.Security` using directive

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

Rebuild `DemoContracts` project and add the file SecureServiceAutoClient.cs to the `DemoClient` project (the file will appear in the root of the project).

8\. Add the following code to the `Main` method of DemoClient.Program.cs:

```cs
using (var application = new ServiceBaseApplication(args, null))
{
  using (var client = new SecureServiceAutoClient("sync://localhost:8080"))
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
private static void exeCommand(SecureServiceAutoClient client, string[] cmd)
{
  try
  {
    var chkPresident = cmd[0].AsBool();
    var name = cmd[1];
    var password = cmd[2];
    var message = cmd[3];
  
    client.Headers.Add(
      new AuthenticationHeader(
        new IDPasswordCredentials(name, password)));

    var response = chkPresident ?
                   client.PresidentEcho(message) :
                   client.Echo(message);

    Console.WriteLine(response);
  }
  catch (Exception error)
  {
    Console.WriteLine(error.ToMessageWithType());
  }
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
Execute the following commands sequentially: "false trump chicago hello", "true trump chicago hello", "false clinton newyork hello", "true clinton newyork hello", "true clinton wrongpwd hello". 
The output will be like

```
Secure service echo: hello
President secure echo: hello
Secure service echo: hello
[NFX.Glue.RemoteException]...
[NFX.Glue.RemoteException]...
```

Therefore, user 'trump' can call both of the server methods `SecureService.Echo` and `SecureService.PresidentEcho`, while user 'clinton' can call only `SecureService.Echo`.