# Include

If you need include some sub-tree in existing configuration you can do this in two ways:

1. Using `NFX.Environment.Configuration.Include()`. 
It completely replaces existing node with another node tree, positioning the new tree in the place of local node. 
Existing node is deleted after this operation completes, in its place child nodes from other node are inserted preserving their existing order. 
Attributes of other node get merged into parent of existing node.

2. Using include pragmas - sections with specified names (`_include` by default) and then calling `NFX.Environment.ConfigNode.ProcessIncludePragmas()` 
which replaces all pragmas with pointed to configuration file content as obtained via the call to file system specified in every pragma. 
If no file system specified then local file system is used. 
Note: this method does not process new include pragmas that may have fetched during this call.

Suppose string variable `confAStr` stores the following configuration section:

```js
staff
{
  employee { name="Scott Black" }
  employee { name="Mark Green" }

  _include
  {
    name=NewDepartment
    file=$"Resources\src_config\incl.laconf"
  }

  _include
  {
    //without name
    file=$"Resources\src_config\incl.laconf"
  }
}
```

The contents of "Resources\src_config\incl.laconf" file are

```js
department
{
  doctor { name="Joe Cooper" }
}
```

Let us also consider another string variable `confBStr` with configuration section

```js
management
{
  manager { name="David Horseman" }
}
```

<br>

After applying of the following lines of code

```cs
var confA = LaconicConfiguration.CreateFromString(confAStr);
var rootConfB = confBStr.AsLaconicConfig();
confA.Include(confA.Root[0], rootConfB);
confA.Root.ProcessIncludePragmas(true);

var result = confA.ToLaconicString();

```

the `result` variable will contain

```js
staff
{
  manager { name="David Horseman" }
  employee { name="Mark Green" }
  
  NewDepartment
  {
    doctor { name="Joe Cooper" }
  }
  
  doctor { name="Joe Cooper" }
}
```

Note that the first "employee" section is completely replaced by the contents of "management" configuration section.