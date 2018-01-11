# Build Information Generator

Generates build information that gets embedded into an assembly image. The information can be accessed using
`NFX.Environment.BuildInformation` class.

Example:

```cs
....
  //Prints BuildInfo for NFX assembly
  Console.WriteLine( BuildInformation.ForFramework );
....
  //Prints the build timestamp for assembly
  var bi = new BuildInformation(assembly);
  var lastModified = bi.DateStampUTC;
....
```

The tool does not take any args. The activation is done via a pre-build step, 
like so:  `"$(TargetDir)buildinfo" > "$(ProjectDir)BUILD_INFO.txt"` where the BUILD_INFO.txt file
must be included in solution as `Embedded Resource`. The file should be cloaked from the source control.






