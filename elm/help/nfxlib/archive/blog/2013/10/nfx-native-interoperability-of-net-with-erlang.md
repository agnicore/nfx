# NFX: Native Interoperability of .NET with Erlang

One of the components included in the NFX library under NFX.Erlang namespace is a set of classes that represent Erlang language types and the connectivity transport that allows to start an Erlang distributed node in a running .NET process and perform demultiplexed communication with other Erlang nodes in the network. This includes ability to redirect I/O, perform RPC calls, etc.

The component can be logically broken down into two parts:
* Erlang types
* Erlang distributed connectivity and message passing

The following basic .NET types (NFX.Erlang namespace) map to the corresponding types in Erlang language, which all implement **NFX.Erlang.IErlObject** interface:
* ErlAtom
* ErlBinary
* ErlBoolean
* ErlByte
* ErlDouble
* ErlList
* ErlLong
* ErlPid
* ErlPort
* ErlRef
* ErlString
* ErlTuple
* ErlVar

Most of these types are structs - i.e. they are merely wrappers around corresponding native types that carry no additional memory or performance overhead. These types are instantiated in the intuitive manner:
```cs
var n = new ErlLong(1000);
var a = new ErlAtom("abc");
var l = new ErlList(n, a, 100, 10.5, "str");
var t = new ErlTuple(n, a, l);
```

Most of Erl native types support implicit casting:
```cs
ErlLong   n1 = 1000;
ErlAtom   a1 = "abc";
ErlString s1 = "efg";
ErlDouble d1 = 10.128;
ErlByte   b1 = 10;
 
int       n2 = n1;
string    a2 = a1;
string    s2 = s1;
double    d2 = d1;
byte      b2 = b1;
```

There are string extension methods that allow to parse strings into Erlang terms:
```cs
IErlObject t0 = "{ok, [{a, 10}, {b, good}, {c, 2.0}]}".ToErlObject();
IErlObject t1 = "{ok, [{a, ~i}, {b, ~w},   {c, ~f}]}".ToErlObject(10, new ErlAtom("good"), 2.0);
ErlList    l0 = "[a,  1, c]".To<ErlList>();
ErlList    l1 = "[a, ~w, c]".To<ErlList>(1);
```

## What can you do with Erlang objects (terms)?
The most useful thing you can do with Erlang types is pattern matching.

The basic idea behind pattern matching is that you can overlay a pattern over an Erlang object, such that the pattern can extract values of sub-objects and bind them with variables.  In order to familiarize yourself with pattern matching, we'll introduce another .NET type, called **ErlVarBind**. It is actually a dictionary mapping variable names to ErlObject's.

In order to illustrate the execution of the code snippets below, we can use the <a href="http://www.linqpad.net/" target="_blank">LINQPad</a> program. Once you install it, open it in the "C# Statement(s)" language mode, and add the reference to **NFX.dll** by right-clicking the query section, going to "Query Properties", and adding **NFX.dll** to the list of "Additional References", and "NFX.Erlang" to the tab of "Additional Namespace Imports". After having done that, type the following code in the query window, and press "F5" to execute:
```cs
var V = new ErlVar("V");           // Create a variable named "V"
var p = "{ok, ~w}".ToErlObject(V); // V is stored as a variable that can be bound
var t = "{ok, 123}".ToErlObject(); // Erlang term to match
 
ErlVarBind b = t.Match(p);         // Match a term against the pattern
                                   // ErlVarBind is a dictionary of bound variables
if (b != null)
    Console.WriteLine("Value of variable {0} = {1}", V.Name, b[V].ValueAsInt);
```

When a match is not successful, **IErlObject.Match()** call returns **null**. Each Erlang object has a set of properties to retrieve their .NET native value. These properties are called **ValueAs{Type}**, where {Type} is .NET specific type, such as **Int**, **Double**, **Decimal**, etc.

Erlang terms can be serialized into Erlang External Binary format using **NFX.Erlang.ErlOutputStream** and **NFX.Erlang.ErlInputStream**:
```cs
var x = "{ok, [{a, 10}]}".ToErlObject();
var s = new ErlOutputStream(x);
Console.WriteLine(s.ToBinaryString());
// Output:  <<131,104,2,100,0,2,111,107,108,0,0,0,1,104,2,100,0,1,97,97,10,106>>
```

Analogously we can deserialize the binary representation back into the corresponding Erlang object:
```cs
var i = new ErlInputStream(new byte[] {131,104,2,100,0,2,111,107,108,0,0,0,1,104,2,100,0,1,97,97,10,106});
Console.WriteLine(i.Read().ToString());
// Output:  {ok,[{a,10}]}
```

## Distributed Erlang: working with remote nodes
In order to illustrate how we can connect a .NET program to an Erlang node, let's fire off an Erlang shell, and give it a security cookie **hahaha** to be used by inter-node authentication:
```erlang
$ erl -sname r -setcookie hahaha
(r@pipit)1>
```

Let's try to connect to this Erlang node and send a message from .NET to Erlang. In order to accomplish this we'll register a named mailbox in the Erlang shell, called "me", and start waiting for incoming message:
```erlang
(r@pipit)1> register(me, self()).
true
(r@pipit)2> f(M), receive M -> io:format("Got message: ~p\n", [M]) end.
```

Now, we can send a message from .NET to this process on the Erlang node:
```cs
var n = new ErlLocalNode("abc", new ErlAtom("hahaha"));
n.AcceptConnections = false;   // Don't accept incoming connections
n.Start();
 
var m = n.CreateMbox("test");
n.Send(m.Self, remoteNode: "r@pipit", toName: "me", new ErlString("Hello!"));
```

What we've done here, we created a .NET local Erlang node called **"abc"** using the same authentication cookie **'hahaha'**, and started it. We instruct the node not to register with the local port mapping daemon (epmd process), so that other nodes cannot connect to it by name. Then we created a named mailbox **"test"**. We used this mailbox in order to send messages (sending messages to remote named processes (mailboxes) requires to have a PID of the sender (m.Self)), make an rpc call, and capture the result.

After executing the code above, the Erlang shell prints:
```erlang
Got message: "Hello!"
ok
```

Let's try to obtain the UTC time from the Erlang node it by making an RPC call from .NET to **erlang:now()** function:
```cs
var r = m.RPC("r@pipit", "erlang", "now", new ErlList());
Console.WriteLine("Remote time: {0}", r.ValueAsDateTime.ToString());
```

We used previously registered mailbox "test" in order to make an rpc call and capture the result. Once the call returned, we output the content to console. .NET outputs:
```
Remote time: 10/9/2013 3:29:47 PM
```

Along with the synchronous RPC, it's possible to do asynchronous calls. Let's illustrate by example (we reuse the variables from preceeding example):
```cs
// The following call is non-blocking - it sends an RPC message
// and returns immediately. Note that ErlList.Empty is analogous to
// "new ErlList()"
m.AsyncRPC("r@pipit", "erlang", "now", ErlList.Empty);
 
// WaitAny call can take several mailboxes and it returns the index
// of the first mailbox, whose queue has some messages waiting
int i = n.WaitAny(mbox);
 
if (i < 0)
{
  Console.WriteLine("Timeout waiting for RPC result");
  goto exit;
}
 
// This call fetches an RPC result from the mailbox
r = m.ReceiveRPC();
 
Console.WriteLine(
  "AsyncRPC call to erlang:now() resulted in response: {0}",
  r.ValueAsDateTime.ToLocalTime());
```

Let's beef this up a little by writing a loop that will pattern match all messages received in our "test" mailbox, and print them out to console. When we receive the atom 'stop' we should exit the loop:
```cs
bool active = true;
var matcher = new ErlPatternMatcher
    {
      {"stop", (p, t, b, _args) => { active = false; return null; } },
      {"Msg",  (p, t, b, _args) => { Console.WriteLine(b["Msg"].ToString()); return null; } },
    };
 
while (active)
{
  m.ReceiveMatch(matcher);
}
 
Console.WriteLine("Done!");
```

Here we introduced another class **ErlPatternMatcher** that takes an array of actions, where the first item is the pattern to match the incoming message against, and the second item is a lambda function receiving four items: the matched pattern, the Erlang object that was used to match against the pattern, the ErlVarBind map containing matched/bound variables from the patterns, and finally the **_args** is that array of optional parameters that can be passed to the ErlMbox.ReceiveMatch() call.

Let's test this in the Erlang shell:
```erlang
(r@pipit)1> {test, ab@pipit} ! "Hello".
 
    LINQPad's output panel prints: "Hello"
```
```erlang
(r@pipit)2> {test, ab@pipit} ! {test, ab@pipit} ! {ok, [1,2,{data, [{a, 12}]}]}.
 
    LINQPad's output panel prints: {ok,[1,2,{data,[{a,12}]}]}
```
```erlang
(r@pipit)2> {test, ab@pipit} ! stop.
 
    LINQPad's output panel prints: Done!
```

The example above illustrated RPC calls from .NET to Erlang. However it is also possible to do the reverse. In current implementation an ErlLocalNode starts a dispatching thread per connection that dispatches incoming Erlang messages to corresponding mailboxes. Messages sent to non-existing mailboxes get silently dropped. One of the internal registered mailboxes created on node's startup is the RPC mailbox called **"rex"**. There's a simple Erlang RPC protocol that .NET implementation supports, which makes it possible for Erlang to invoke static member functions in .NET. Here we illustrate a call from the Erlang shell into the .NET node in order to obtain the local time from .NET:
```erlang
(r@pipit)3> f(Time), {ok, Time} = rpc:call(ab@pipit, 'System.DateTime', 'UtcNow', []), calendar:now_to_local_time(Time).
{{2013,10,8},{1,24,26}}
```

Having illustrated RPC, we now show how to do I/O redirection. Suppose you do an RPC call from .NET to Erlang, and you want to make sure that all output printed by that call via **io:format()** and alike functions is sent back to .NET. This is accomplished by the fact that .NET node runs another server thread that polls for data in a special mailbox registered by name **"user"**. This mailbox is also accessible via **ErlLocalNode.GroupLeader**. All RPC calls by default pass that mailbox information, so that remote nodes could deliver the output there. Let's illustrate:
```cs
var n = new ErlLocalNode("d") {
    OnIoOutput = (_encoding, output) =>
        Console.WriteLine("<I/O output>  ==> Received output: {0}", output)
};
  
n.Start();
var m = n.CreateMbox("test");
var c = n.Connection("r@pipit");
var r = m.RPC(c.RemoteNode.NodeName, "io", "format", new ErlList("Hello world!"));
Console.WriteLine("Result: {0}", r.ToString());
```

When we execute this code, here's what gets printed:
```
<I/O output>  ==> Received output: "Hello world!"
Result: ok
```

## Runtime configuration
Erlang node implementation supports a powerful concept of NFX framework configuration. In order to auto-configure a .NET application to start an Erlang node at application startup, we need to define a starter configuration section, and provide node startup details:
```
nfx
{
  starters {
    starter{ name="Erlang" type="NFX.Erlang.ErlApp" }
  }
   
  erlang
  {
    cookie="hahaha"

    node="me" {
      trace="wire"
      accept=true
      address="localhost" // address="127.0.0.1:1234"
      tcp-no-delay=true
      tcp-rcv-buf-size=4096
      tcp-snd-buf-size=4096
    } 

    node="r@localhost" {
      tcp-no-delay=false
      tcp-rcv-buf-size=100000
      tcp-snd-buf-size=100000
    }
  }   
}
```

The "starters" section in NFX contains a list of static starter types that implement **IApplicationStarter** behavior. For Erlang node that is implemented by the "NFX.Erlang.ErlApp" type.

Next we define the "erlang" section that tells to use cookie "hahaha" for connecting to distributed Erlang nodes. It also defines a node "me" to be used as the local node name (since it doesn't have the "@hostname" suffix), which will be accessible through a static singleton variable **ErlApp.Node**. Also at startup the local node "me" will connect to remote node "r@localhost", whose connection configuration details are customized under 'node="r@localhost'.

The local node "me" will register with EPMD daemon, and accept incoming connections from other nodes ("accept=true"), it will enable debug tracing to print wire-level messages ("trace=wire") if ErlApp.Node.OnTrace event has been set in the application code.

The NFX application startup code and sample simplistic implementation that prints out messages received by the "test" mailbox then looks something like this:
```cs
static void Main(string[] args)
{
  Configuration argsConfig = new CommandArgsConfiguration(args);

  using(new ServiceBaseApplication(args, null))
    run();
}
 
static void run()
{
  var mbox = ErlApp.Node.CreateMbox("test");

  while (App.Active)
  {
    var result = mbox.Receive(1000);
    if (result != null)
      Console.WriteLine("Mailbox {0} got message: {1}", mbox.Self, result);
  }
}
```

## Conclusion
NFX implementation of Erlang terms and distributed transport provides a rich set of primitives needed to communicate with applications running Erlang nodes very efficiently with minimal memory with processing overhead.

The implementation takes advantage of modern C# concepts that include Linq, Enumerators, and other paradigms that make writing distributed systems a very pleasant experience.

NFX Erlang is a complete rewrite of its predecessor <a href="https://github.com/saleyn/otp.net" target="_blank">otp.net</a>. Current version eliminates all deficiencies of the former inherited from initial auto-conversion from corresponding Java code, contains a much cleaner and simpler conceptual model, and gives a .NET programmer a very powerful tool of exploring Erlang interoperability.

---
Sergey Aleynikov  
October 8, 2013