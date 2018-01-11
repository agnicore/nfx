# Application Configuration

Configuration files are mainly used to inital setup of applications.
To automate application configuration on startup follow a few steps:

1. Add 'your-assembly-name.laconf' configuration file in the project root. 
Be sure that the file added with `BuildAction: None`, `Copy to Output Directory: Copy always` options.

2. Add `NFX.ApplicationModel.ServiceBaseApplication` wrapper in program entry point:

```cs
static void Main(string[] args)
{
  using (var application = new ServiceBaseApplication(args, null))
  {
    // application bootstrapping and launch code
  }
}
```

This pattern unifies the development of many application kinds: console, services, web, WinForms etc.