# Security

Let see on the example how interaction between nodes with some security requirements can be implemented with Glue technology.
Here is configuration on the server node. On the client node only bindings section is required.

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
        server-transport
        {
          rcv-buf-size=131072
          snd-buf-size=131072
          rcv-timeout=15000
          snd-timeout=15000
        }
      }
    }
        
    servers
    {
      server
      {
        node="sync://localhost:8080"
        contract-servers="Glue.Server.Services.SecureService, Glue.Server"
      }
    }
  }
```

In this example some security requirements are set. 
Here are two registered users with different permission levels (see `NFX.Security.AccessLevel` that sets level of access granted to user for certain permission).
  
```js
security
{
  users
  {
    user
    {
      name="Clinton"
      description="Bill Clinton"
      id="clinton"
      password="89c246298be2b6113fb10ba80f3c6956" // phash 'billy'
      rights
      {
        specialized
        {
          superman  {level=1}
          president {level=1}
        }
        gov
        {
          executive {level=0}  // he is not current president
        }
      }
    }
   
    user
    {
      name="Trump"
      description="Donald Trump"
      id="trump"
      password="10189466c646898c1fcf2363b5cf038d" // phash 'chicago'
      rights
      {
        specialized
        {
          superman  {level=1}
          president {level=1}
        }
        gov
        {
          executive {level=1} // he is current president
        }
      }
    }
  }
}
```

As it was mentioned above contract should be specified with Glued attribute. AuthenticationSupport attribute indicates that contract supports authentication using AuthenticationHeader. When header is passed then Glue server will use its data to set user context through Application.SecurityManager. If this attribute not set then Glue runtime will ignore AuthenticationHeader that marshalls user authentication information. AdHocPermission attribute represents a permission check instance which is a-typical and is based on string   arguments.

```csharp
[Glued]
[AuthenticationSupport]
[AdHocPermission("specialized", "superman", AccessLevel.VIEW)]
public interface ISecureService
{
  string Echo(string message);

  // This method can only accessed by superman
  // who is also a president
  [AdHocPermission("specialized", "president", AccessLevel.VIEW)]
  string PresidentEcho(string message);
}

// Service on the server
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

Implementation of client class is received with calling

gluec.exe <assembly> /o cl-suffix="AutoClient"

where <assembly> - path to CLR assembly to scan for types marked with Glued attribute and cl-suffix - class suffix gets attached at the end of client class name.

```csharp
public class SecureServiceAutoClient : ClientEndPoint,
  @Glue.@Contracts.@Services.@ISecureService
{
  private static TypeSpec   s_ts_CONTRACT;
  private static MethodSpec @s_ms_Echo_0;
  private static MethodSpec @s_ms_PresidentEcho_1;

  static SecureServiceAutoClient()
  {
    var t = typeof(@Contracts.@Services.@ISecureService);
    s_ts_CONTRACT = new TypeSpec(t);
    @s_ms_Echo_0 = new MethodSpec(t.GetMethod("Echo",
                                    new Type[]{ typeof(@System.@String) })
                                  );
    @s_ms_PresidentEcho_1 = new MethodSpec(t.GetMethod("PresidentEcho",
                                             new Type[]{ typeof(@System.@String) })
                                           );
  }

  ...

  public @System.@String @Echo(@System.@String  @message)
  {
    var call = Async_Echo(@message);
    return call.GetValue<@System.@String>();
  }

  public CallSlot Async_Echo(@System.@String @message)
  {
    var request = new RequestAnyMsg(s_ts_CONTRACT,
                                    @s_ms_Echo_0,
                                    false,
                                    RemoteInstance,
                                    new object[]{@message});
    return DispatchCall(request);
  }

  // two similar methods for ISecureService.PresidentEcho
}
```

`ClientEndPoint` class represents an ancestor for client classes that make calls to server endpoints. 
This and descendant classes are thread safe only for making non-constructing/destructing remote calls, 
unless `ReserveTransport` flag is set to true in which case no operation is thread safe. 
This class is not thread safe in general, however Glue allows for concurrent remote calls via the same endpoint 
instance if the following conditions are met:
* The endpoint instance has not reserved its transport (`ReserveTransport`=false)
* Either remote contract is stateless or none of the concurrent calls are constructing/destructing remote instance 

The second condition ensures that stateful remote instance is consistent, otherwise operations may get executed out-of-order in the multithreaded scenario.
`TypeSpec` and `MethodSpec` represent type and method specifications for marshalling contract types between glued peers correspondingly.
`DispatchCall` is method of `ClientEndPoint` class, it dispatches a call into binding passing message through client inspectors on this endpoint.
`CallSlot` represents a class that is immediately returned after transport sends RequestMsg. This class provides `CallStatus` and `RequestID` properties where the later is used to match the incoming `ResponseMsg`. `CallSlots` are kinds of "spirit-less" mailboxes that keep state about the call, but do not possess any threads/call events. Working with `CallSlots` from calling code's existing thread of execution is the most efficient way of working with Glue (in high load cases), as it does not create extra object instances for asynchronous coordination and continuation.
`CallSlot.GetValue` method returns a value from the other side, it gets the response message and checks it for errors, throwing `RemoteError` exception if one came from server. 
Accessing this property blocks calling thread until either `ResponseMsg` arrives or timeout expires. Accessing this method for one-way methods throws exception.
`RequestMsg` represents request message from client to server that contains contract type, method specification and invocation arguments which are included as object array. Although the most convenient and simple, this way of working with glue is slower than using its typed version which needs to be derived-from for every method (that allows to avoid boxing).
So fragment of code with `SecureServiceAutoClient` usage may look like:

```csharp
try
{
  using (var client =
               new SecureServiceAutoClient("sync://localhost:8080"))
  {
    client.Headers.Add(new AuthenticationHeader(
                         new IDPasswordCredentials(user, password)));

    if (!isPresident)
      response = client.Echo("test");
    else
      response = client.PresidentEcho("test");

    System.Console.WriteLine(response);
  }
}
catch (Exception error)
{
  System.Console.WriteLine(error.Message());
}
```

Now due to security settings in the configuration and the contract there will be errors if you tries to log in neither as "clinton" or "trump", 
or tries to send message to server as "clinton" with isPresident=true. 
If you log in as "clinton" you can use isPresident=false only and will get "Secure service echo: test" as a result. As "trump" you can use both values of isPresident and will get both corresponding responses.
In this fragment `AuthenticationHeader` marshalls user authentication information.