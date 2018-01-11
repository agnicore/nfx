# Setup Application Entry Point

The app container usually gets set-up at the process entry point like so:

```csharp
class Program
{
  static void Main(string[] args)
  {   
    try
    { 
      using(new ServiceBaseApplication(args, null))
      {
          ConsoleUtils.Info("Server is running. ");
  
          ConsoleUtils.Info("Glue servers:");
          foreach(var s in ServiceBaseApplication.Glues.Servers)
            Console.WriteLine("  " + s);
          
          Console.WriteLine();
          ConsoleUtils.Info("Press <enter> to end server program");
          Console.ReadLine();
      }
    }
    catch(Exception error)
    {
      ConsoleUtils.Error(error.ToMessageWithType());
      Environment.ExitCode = -1;
    }
  }
}
```

This pattern unifies the development of many application kinds: console, services, web, WinForms etc. 
Unlike, bare .NET process that lacks a concept of logging, component registry and other aforementioned services **NFX** app container provides all of that. 
Consequently, one may rely on many services that always exist in the execution context. 
For example: any application written in **NFX** has security and permissions and session management (a kin to web sessions), even console apps. 

In cases when specific “hard-coded” services are not needed, the app container injects a NOP (no operation) implementor of the service, thus an app developer never needs to check if say logging or session state is injected/available. 

Application Component is a base class for many services (such as logging etc..), it keeps an internal record and allows callers to enumerate all components in the process. 
Another powerful feature is instrumentation and external parameters (applied by decorating properties with attributes) which can be controlled by external processes, so this allows for remote process/component  administration in runtime without any “debugger black magic” (such as interrupts and direct memory access). 

Application container may be viewed as the root of dependency injection container tree with some hard-coded nodes - this was done on purpose to significantly simplify the design. 
The components are injected from Configuration or by hand (allocated by code). 
The practical benefit of this approach is such, that in 90% of applications just specifying config file is enough to set up a complex process structure.
