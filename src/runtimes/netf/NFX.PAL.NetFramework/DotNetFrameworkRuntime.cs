using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFX.PAL;

namespace NFX.PAL.NetFramework
{
  /// <summary>
  /// Represents .NET Framework runtime
  /// </summary>
  public static class DotNetFrameworkRuntime
  {
    /// <summary>
    /// Binds the .NET Framework implementation to Platform Abstraction Layer
    /// This method must be called only once at the assembly entry point
    /// </summary>
    public static void Init()
    {
      IPALImaging imaging = null;//not provided yet

      var machine = new PALMachineInfo();
      var fs = new PALFileSystem();


      NFX.PAL.PlatformAbstractionLayer.____SetImplementation(
                   ".NET Framework",
                   imaging,
                   machine,
                   fs);
    }
  }
}
