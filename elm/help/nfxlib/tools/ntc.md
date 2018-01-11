# NFX Template Compiler/Transpiler


Transpiles the source template in particular syntax into another syntax. The tool takes the compiler
type via `-c|-compiler` switch. The default `TextCSTemplateCompiler` generates CS source code 
based on NTC syntax. See [Specifications](/specs) section for NTC template details.


A typical Visual Studio pre-build step use case to generate template pages:

```css
"$(SolutionDir)..\Output\$(ConfigurationName)\ntc" "$(ProjectDir)\Pages\*.htm" -r -ext ".auto.cs" -src
```

Type`ntc -?` for help:

```css
 Usage:
   ntc source_path [/h | /? | /help]
              [/options | /o[asm-file= file_name]
                            [compile-code= true|false]
                            [base-type= base_type_name]
                            [namespace= namespace_name]
                            [ref= assembly_name0[;assembly_nameX...]]
                            [ref-path= path]
              ]
              
              [/r | /recurse]
              [/c | /compiler  fully_qualified_type_name]
              [/src]   
              [/ext file_extension]

 source_path - path to template source files, may include wildcard

Options:

 /h | /help | /? - displays help message 
 /o | /options - specifies compiler options.
                 
    asm-file - creates an assembly file on disk
    compile-code - performs code compilation   

    base-type - specifies default base type name which has to be fully qualified but without
       assembly namespace - fully qualified namespace name
    ref - adds assembly reference, may contain multiple names separated by ";" character
    ref-path - additional path used for referenced assemblies location
         
 /r | /recurse - walk subdirectory structure as well
 /c | /compiler - uses specified compiler. If omitted then TextCSTemplateCompiler is used
 /ext - specifies file extension to use for source files. When omitted, default language 
        extension will be appended to source file name
 /src - writes compiled source files to disk

Examples:

  ntc "c:\templates\*.tpl" -r -src 
Compiles all files with "tpl" extension in specified folder and all subfolders
using default TextCSTemplateCompiler generating C# source files on disk

  ntc "c:\templates\*.tpl" -r -o compile-code=true 
        ref-path="c:\mylibs"
        ref="System.Web.dll;NFX.Web.dll"
        asm-file="c:\mylibs\Templates.dll" 
        
Compiles all files with "tpl" extension in specified folder and all subfolders
using default TextCSTemplateCompiler generating assembly "Templates.dll" that
contains compiled template types. Adds "System.Web" and "NFX.Web" referenced assemblies
providing reference searchpath at "c:\mylibs"
```
