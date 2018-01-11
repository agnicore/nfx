# Bindings

The Glue has concept of binding just like WCF does. 
Binding tells you what kind of protocol and what kind of message exchange pattern is going to be used between the endpoints. 
The Glue has a family of bindings called native bindings because they are native to Glue and are implemented right in Glue project. There are two of them. 
First one is called MPX which stands for multiplexing binding that is bidirectional  socket. 
It sends information and it doesn’t block the socket channel until the result comes back for the two way calls. 
It multiplexes one socket for multiple calls. The another binding is called Sync binding which is similar to MPX only it is 100% blocking synchronous socket. 
The best performance you can get for a large transfers is through synchronous binding because synchronous model programming is more efficient than asynchronous 
one for a special kinds of operations. 
Both of these binding (MPX and Sync) use Slim Serializer that is highly optimized dynamically compiling expression tree based serializer, 
it allows to serialize complex graphs, including cyclical, acyclical, network like, self-referencing with transitive references graphs.
The Glue is configured from application container - "glue" section is used. In this section a lot of parameters can be tuned, 
e.g. message tracing, receive buffer window sizes, timeouts and etc., inspectors can be injected, but basically thing to do here is to setup bindings, give them names, define types that handle those binding.
Also servers are defined here in corresponding section. Node attribute defines what binding should be used and what address should be listen. 
There is specified what servers should be exposed on that address.


**Example 2:**

```js
glue
{
  bindings
  {
    binding
    {
      name="sync"
      type="NFX.Glue.Native.SyncBinding" 
    }
  }

  servers
  {
    server
    {
      name="sync"
      node="sync://*:8000"
      contract-servers="TestServer.Glue.JokeCalculatorServer, TestServer"
    }
  }
}
```

It should be noted that Glue Client Compiler gluec.exe tool can be used in command line to automatically generate client classes for corresponding contracts.