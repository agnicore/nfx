# Command Args Configuration

You can create special type of configuration `NFX.Environment.CommandArgsConfiguration` based on arguments supplied from command line which is `string[]`. 
Arguments start with either "/" or "-" prefix. 
If any argument is not prefixed then it is written as an auto-named (`?index`) attribute node of the root with its value set, otherwise a section (under root) with argument's name is created. 
Any argument may have options. Any option may either consist of name or name value pair delimited by "=". 
Argument options are written as attribute nodes of their corresponding sections. 
If option value specified without name (without "=") then option is auto-named.

Let `argsStr` variable contains the following command args string:

**tool.exe c:\input.file d:\output.file -compress level=10 method=zip -shadow fast swap=1024 /large**

The code

```cs
var args = argsStr.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
var conf = new CommandArgsConfiguration(args);
var result = conf.ToLaconicString();
```

will transform it into laconic configuration:

```js
args
{
  ?1=tool.exe
  ?2="c:\\input.file"
  ?3="d:\\output.file"
  compress
  {
    level=10
    method=zip
  }
  shadow
  {
    ?1=fast
    swap=1024
  }
  large
  {
  }
}
```