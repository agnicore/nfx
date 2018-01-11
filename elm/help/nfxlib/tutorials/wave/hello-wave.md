# Hello WAVE
NFX.WAVE represents "(W)eb(A)pp(V)iew(E)nhanced" web server which provides DYNAMIC web site services.
This server is not meant to be exposed directly to the public Internet, rather it should be used as an application server behind the reverse proxy, such as NGINX. 
This server is designed to serve dynamic data-driven requests/APIs and not meant to be used for serving static content files (although it can). 
The implementation is based on a lightweight HttpListener that processes incoming Http requests via an injectable `WorkDispatcher` which governs the threading and `WorkContext` lifecycle.

Below is step-by-step guide to create the simplest NFX.Wave application that outputs "Hello WAVE!" message to your browser.

1. Create new C# Console Application project `HelloWorld`.

2. Add reference to **NFX** (e.g. <a href="https://www.nuget.org/packages/NFX" target="_target">via NuGet</a>).

3. Add the following code to `Program.cs`:
   ```cs
   using System;
   using NFX.ApplicationModel;
   using NFX.Wave;
   
   namespace HelloWorld
   {
     class Program
     {
       static void Main(string[] args)
       {
         try
         {
           using (var app = new ServiceBaseApplication(args, null))
           using (var server = new WaveServer())
           {
             server.Configure(null);
             server.Start();
             Console.WriteLine("server started...");
             Console.ReadLine();
           }
         }
         catch (Exception error)
         {
           Console.WriteLine("Critical error:");
           Console.WriteLine(error);
           Environment.ExitCode = -1;
         }
       }
     }
   }
   ```
Here we create web server instance (`WaveServer`) in the scope of application container (`ServiceBaseApplication`). 
In this example the server is configured with application-level configuration - `server.Configure(null)` (to be added below), 
but one can configure web server with any specific configuration. 

4. Create project folder `Pages` and add a file `Pages\Index.nht` with the following contents:
  ```html
  #<laconf>
    compiler
    {
      base-class-name="NFX.Wave.Templatization.WaveTemplate"
      namespace="HelloWorld.Pages"
    }
  #</laconf>
  
  <!DOCTYPE html>
  <html>
    <head>
    </head>
    <body>
      <div id="root">Hello WAVE!</div>
    </body>
  </html>
  ```
This is the simplest [NTC template](/specs/template.html) of application start page. 
The code in `#<laconf>` brackets contains directives for [NFX Template Compiler](/tools/ntc.html) which generates C# class from the template.

5. To obtain a file with C# class from the `Index` page template you can choose from two variants: explicitly run [ntc tool](/tools/ntc.html) with proper parameters or add following line (fix path to ntc tool if it is necessary) to project pre-build events
```
"$(SolutionDir)packages\NFX.3.4.0.1\tools\ntc" "$(ProjectDir)Pages\*.nht" -r -ext ".auto.cs" /src
```
Add the resulting file (e.g. Index.nht.auto.cs) with `Index` class to the project.

6. Create project folder `Controllers` and add a class `Controllers\Hello.cs`:
   ```cs
   using NFX.Wave.MVC;
   using HelloWorld.Pages;
   
   namespace HelloWorld.Contollers
   {
     public class Hello : Controller
     {
       [Action]
       public object Index()
       {
         return new Index();
       }
     }
   }
   ```

7. Add configuration file `HelloWorld.laconf` with the following contents:
   ```js
   application
   {
     wave
     {
       server
       {
         prefix { name="http://localhost:8070/" }
         dispatcher
         {
           handler
           {
             type="NFX.Wave.Handlers.MVCHandler, NFX.Wave"
             type-location
             {
               assembly="HelloWorld.exe"
               ns { name="HelloWorld.Controllers" }
             }
             match { path="/{type=Hello}/{mvc-action=Index}" }
           }
         }
       }
     }
   }
   ```
Make sure that Build Action is set to None and Copy to Output Directory is set to Copy Always.

In `prefix` section we define the address where we can send a request to the server. 
In `handler` section we define type of the handler, assembly where corresponding controllers are placed and pattern of path to controller and action.

8. Run the `HelloWorld` application, open browser and type `http://localhost:8070/hello/index` or just `http://localhost:8070` in the address bar.