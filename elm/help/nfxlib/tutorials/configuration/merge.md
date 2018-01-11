# Merge

While working with configurations you can merge them together with `NFX.Environment.Configuration.CreateFromMerge()`. 
It creates a new configuration from ordered merge result of two other nodes which can be from different configurations. 
The first operand is a "base" node that data is defaulted from. 
And the second one contains overrides/additions of/to data from base node. 
Also you can influence merging process with `_override` pragma.

Suppose `confAStr` and `confBStr` string variables store the following two configuration sections correspondingly:

```js
root
{
  section-a
  {
    husband="John Smith" {}
    wife="Helen Black" {}
    child
    {
      name="James"
      age=10
    }
    child
    {
      name="Ann"
      age=13
    }
  }

  section-b
  {
    _override=attributes
    name="Mark Johnson" {}
    age=32
  }
  
  //section-c="Exception will be thrown if uncomment." { _override=fail }

  section-d="This can not be overridden but no exception will be thrown." { _override=stop }

  section-e
  {
    _override=replace
    name="Henry Gold" {}
    age=99
  }

  section-f
  {
    _override='sections'
    name="David Crowman" {}
    age=18
  }
}
```

```js
root
{
  section-a
  {
    wife="Olga Smith" {}
    child
    {
      name="Mary" //try to put "Ann" instead of
      age=8
    }
    city="New York"
  }

  section-b
  {
    name="Lisa Stone" {}
    age=10
  } 

  section-c="Could cause failure while merging." {}

  section-d="This value will be ignored." { name="Peter"{} }

  section-e
  {
    name="Harold Snow" {}
    age=25
  }

  section-f
  {
    name="Jane Wood" {}
    age=50
  }
}
```

<br>

After applying following code

```cs
var confA = LaconicConfiguration.CreateFromString(confAStr);
var confB = LaconicConfiguration.CreateFromString(confBStr);
var conf = new MemoryConfiguration();
conf.CreateFromMerge(confA.Root, confB.Root);

var result = conf.ToLaconicString();
```

the variable `result` will contains merged configuration:

```js
root
{
  section-a
  {
    city="New York"
    husband="John Smith" {}
    wife="Olga Smith" {}
    child
    {
      name=James
      age=10
    }
    child
    {
      name=Ann
      age=13
    }
    child
    {
      name=Mary
      age=8
    }
  }
  section-b
  {
    _override=attributes
    age=10
    name="Mark Johnson" {}
  }
  section-d="This can not be overridden but no exception will be thrown."
  {
    _override=stop
  }
  section-e
  {
    age=25
    name="Harold Snow" {}
  }
  section-f
  {
    _override=sections
    age=18
    name="Jane Wood" {}
  }
  section-c="Could cause failure while merging." {}
}
```

