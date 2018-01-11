# Overview

One of the main purposes of any configuration process is the ability to fill entities with values stored in configuration file. 
With **NFX** configuration one can do it by placing `NFX.Environment.ConfigureAttribute` on class members and then calling `ConfigAttribute.Apply()` to automatically supply your class members with values taken from config. 
Entities that should have more precise configuration (e.g. recursive configuration by **NFX** infrastructure etc.) must implement interface

```csharp
public interface IConfigurable
{
  void Configure(IConfigSectionNode node);
}
```

It is preferable to use special LACONIC configuration format which gives full flexibility, but it is also possible to use XML or JSON formats which are fully supported in **NFX**.

Below are some examples of **NFX** configuration features. For more details, please see following links

**Example 1:**

The following configuration

```js
root
{
  person
  {
    data
    {
      salary=3000
      phone-number='8-901-900-00-00'
      name='John'
      DOB='12/05/1980'
    }
  }
}
```

being applied to instance of a class

```csharp
[Config("person/data")]
public class Person
{
  [Config]
  private int m_Salary;

  [Config("$phone-number")]
  protected string m_Phone;

  [Config]
  public string Name { get; set; }

  [Config]
  public DateTime DOB { get; set; }

  [Config("$height-cm", 180)]
  public int Height { get; set; }
}
```

by the following two lines of code

```csharp
var person = new Person();
ConfigAttribute.Apply(person, conf.Root);
```

results in filling all class instance fields and properties which are marked with `[Config]` attribute with the config values. Height property is absent in config node thus it will be initiated by default value 180. 
Notice how Config attributes allows for attribute name overrides, when omitted the attribute name is derived from a member name having member prefix stripped-off.

**Example 2:**
Truly impressive configuration flexibility is provided by its **merge** and **include** features. 
As the names implies, the latter provides merging operation in a natural way. 
Following code performs merge and include operations correspondingly:

```csharp
var conf = new MemoryConfiguration();
conf.CreateFromMerge(confA.Root, confB.Root);
```

and

```csharp
confA.Include(confA.Root["section-name"], confB.Root)
```

Merging operation can also be controlled by `_override` pragma.

**Example 3:**

Navigation within configuration nodes can be achieved by following code examples

```csharp
conf.Root["section-name"]
conf.Root["section-name"]["sub-section-name"]
conf.Navigate("/$a").ValueAsInt()
conf.Navigate("/contacts/phone/$[0]").Value
conf.Root.AttrByName("meduza")
```

**Example 4:**

Custom variables can also be declared and evaluated for future reuse within configuration:

js
root
{
  gv
  {
    paths
    {
      home-root=$(~ENV_HOME_ROOT)
      ds-mysql="NFX.MySQL"
    }
  }
  
  type="$(/gv/paths/$ds-mysql)"
  home="$(/gv/paths/$home-root)"
  nowis=$(::now) // "::" - macro
}
```

Here `$(~ENV_HOME_ROOT)` resolves system environment variable, 
`"$(/gv/paths/$ds-mysql)"` and `"$(/gv/paths/$home-root)"` are resolved into some variables defined above in the code