# Attributes

One of the main purpose of any configuration file is to fill class istances with values stored in it. 
With *.laconf configuration this can be achieved by placing `NFX.Environment.ConfigureAttribute` on class members and then calling `ConfigAttribute.Apply()` to automatically supply your class members with config values. 
Also one can implement `NFX.Environment.IConfigurable` interface to achieve more precise control over filling class from configuration node.

Suppose that we want to fill instance of the following class

```cs
[Config("Person/data")]
public class Person
{
  private const string PERSON_DATA = "Person data:";
  
  [Config("$private-salary")]
  private int m_salary;
  
  [Config("$protected-phone")]
  protected string m_Phone;
  
  [Config("$public-name")]
  public string Name { get; set; }
  
  [Config("$DOB")]
  public DateTime DOB { get; set; }
  
  [Config("$height", 180)]
  public int Height { get; set; }
  
  [Config("extra/$enum-country")]
  public Countries Country { get; set; }
  
  [Config]
  public byte[] AgesOfChildren;
  
  [Config("extra/address")]
  public IConfigSectionNode Address { get; set; }
}

public enum Countries { USA, Canada, Japan }
```

with the values store in the following configuration

```js
root
{
  Person
  {
    data
    {
      private-salary=3000
      protected-phone='8-901-900-00-00'
      public-name='John'
      DOB='1980/12/05'
      // height is absent and will be initiated with default value
      ages-of-children='F,3,8' // hexadecimal only!
      extra
      {
        enum-country=USA
        address
        {
          zipcode=10010 {}
          city='New York'
        }
      }
    }
  }
}
```

This can be done by the following code

```cs
var conf = LaconicConfiguration.CreateFromString(str);
var person = new Person();
ConfigAttribute.Apply(person, conf.Root);
```

Here `str` variable contains config section above.