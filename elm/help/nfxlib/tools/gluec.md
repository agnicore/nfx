# NFX Glue Client Compiler

Extracts interfaces denoted by `[Glued]` attribute and generates the source code for call proxy.
The compiler is injectable via `-c|-compiler` switch, the default is `CSharpGluecCompiler`.

Example contract:
```cs
/// <summary>
/// Represents a contract for working with remote entities using terminal/command approach
/// </summary>
[Glued]
[AuthenticationSupport]
[RemoteTerminalOperatorPermission]
[LifeCycle(ServerInstanceMode.Stateful, SysConsts.REMOTE_TERMINAL_TIMEOUT_MS)]
public interface IRemoteTerminal
{
    [Constructor]
    RemoteTerminalInfo Connect(string who);
 
    string Execute(string command);
 
    [Destructor]
    string Disconnect();
}
```

generates the proxy:

```cs
 var node = new Node("async://quad:7311"); 
 var console = new RemoteTerminalClient( node );
 console.Connect("Jack Lowery");
 
 Console.WriteLine("The time on connected node is: " + console.Execute("time"));
 
 console.Disconnect();
```


Type`gluec -?` for help.

```css
 Usage:
   gluec assembly [/h | /? | /help]
              [/options | /o[out= file_name]
                            [fpc= true|false]
                            [ns-suffix= namespace_suffix]
                            [cl-suffix= class-suffix]
              ]
              
              [/f | /flt | /filter namespace_list]
              [/c | /compiler  fully_qualified_type_name]
                                                                               
assembly - path to CLR assembly to scan for types marked with [Glued] attribute

Options:

/h | /help | /? - displays help message 
/o | /options - specifies compiler options.
                 
          out - output path, if omitted then assembly path is used
          fpc - file-per-contract, when true puts each contract in a separate source code file   
          ns-root - namespace root name, if set replaces original contract namespace name
          ns-suffix - namespace suffix gets attached at the end of the original contract ns name
          cl-suffix - class suffix gets attached at the end of client class name
 
 /f | /flt | /filter - a ';'=delimited list of namespaces to process contracts from the assembly
 /c | /compiler - a fully qualified compiler type name, if omitted CSharpGluecCompiler is used

Examples:

  gluec "c:\bin\BusinessLogic.dll" 
        /o out="c:\bin\contracts"
           fpc=false
           ns-suffix="Callers"
Compiles contracts from assembly into specified out path all in one source CSharp file.
Namespaces are suffixed with "Callers"
```

