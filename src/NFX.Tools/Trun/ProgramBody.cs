/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2018 Agnicore Inc. portions ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using NFX;
using NFX.ApplicationModel;
using NFX.Scripting;
using NFX.Environment;
using NFX.Templatization;
using NFX.IO;

namespace NFX.Tools.Trun
{
    public static class ProgramBody
    {
        public static void Main(string[] args)
        {
          try
          {
           using(var app = new ServiceBaseApplication(true, args, null))
           {
             Console.CancelKeyPress += (_, e) => { app.Stop(); e.Cancel = true;};

             System.Environment.ExitCode = run(app);
           }
          }
          catch(Exception error)
          {
           ConsoleUtils.Error(error.ToMessageWithType());
           System.Environment.ExitCode = -1;
          }
        }


        private static int run(ServiceBaseApplication app)
        {
          var config = app.CommandArgs;

          ConsoleUtils.WriteMarkupContent( typeof(ProgramBody).GetText("Welcome.txt") );


          if (config["?"].Exists ||
              config["h"].Exists ||
              config["help"].Exists)
          {
             ConsoleUtils.WriteMarkupContent( typeof(ProgramBody).GetText("Help.txt") );
             return 0;
          }


          var assemblies = config.Attributes
                                 .Select( a => Assembly.LoadFrom(a.Value))
                                 .ToArray();

          if (assemblies.Length==0)
          {
            ConsoleUtils.Error("No assemblies to run");
            return -2;
          }


          Console.ForegroundColor =  ConsoleColor.DarkGray;
          Console.Write("Platform runtime: ");
          Console.ForegroundColor =  ConsoleColor.Yellow;
          Console.WriteLine(NFX.PAL.PlatformAbstractionLayer.PlatformName);
          Console.ForegroundColor =  ConsoleColor.Gray;

          var hnode = config["host"];
          var rnode = config["r", "runner"];

          var errors = 0;
          using(var host =  FactoryUtils.MakeAndConfigure<IRunnerHost>(hnode, typeof(TestRunnerConsoleHost)))
          {
            Console.ForegroundColor =  ConsoleColor.DarkGray;
            Console.Write("Runner host: ");
            Console.ForegroundColor =  ConsoleColor.Yellow;
            Console.WriteLine(host.GetType().DisplayNameWithExpandedGenericArgs());
            Console.ForegroundColor =  ConsoleColor.Gray;

            foreach(var asm in assemblies)
            {
              using(var runner =  FactoryUtils.Make<Runner>(rnode, typeof(Runner), args: new object[]{asm, host, rnode}))
              {
                Console.WriteLine("Assembly: {0}".Args(asm));
                Console.WriteLine("Runner: {0}".Args(runner.GetType().DisplayNameWithExpandedGenericArgs()));
                Console.WriteLine();

                runner.Run();
                errors += host.TotalErrors;

                Console.WriteLine();
                Console.WriteLine();
              }
            }
          }//using host

          return errors > 0 ? -1 : 0;
        }



    }
}
