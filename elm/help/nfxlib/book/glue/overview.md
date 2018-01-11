# Overview

Glue is based on contracts - must be specified with GluedAttribute and has pluggable bindings that define protocol and serialization, 
it has security part that can be used in declarative or imperative form (permissions can be assigned to whole contract or separate methods directly). 
With changing `ServerInstanceMode` parameter of `LifeCycleAttribute` for contracts the Glue proposes opportunity to implement stateless or stateful server/client paradigm.

**Example 1:**

Service contract:

```csharp
[Glued]
[LifeCycle(ServerInstanceMode.Stateful, 20000)] // 20000-timeout to destruct in case of the loss of connection
[AuthenticationSupport]
public interface IJokeCalculator
{
  [Constructor]
  void Init(int value);

  [SultanPermission(250)] //some additional permission
  int Add(int value);
  
  int Sub(int value);

  [Destructor]
  int Done();
}
```

Service contract implementation:

```csharp
[NFX.Glue.ThreadSafe]
public class JokeCalculatorServer : IJokeCalculator
{
  private int m_Value; //state is retained between calls

  public void Init(int value)
  {
    m_Value = value;
  }

  public int Add(int value)
  {
    m_Value += value;
    return m_Value;
  }
}
```

Client:

```csharp
public class JokeCalculatorClient : ClientEndPoint, IJokeCalculator
{
  ...
  public int Add(int value)
  {
    // call to server and return result
  }
}
```

```csharp
var node = new Node("sync://192.168.1.23:8000");
using (var cl = new JokeCalculatorClient(node))
{
  cl.Init(234);
  cl.Add(3);
  Assert.AreEqual(237, cl.Done()); // deallocate instance
}
```