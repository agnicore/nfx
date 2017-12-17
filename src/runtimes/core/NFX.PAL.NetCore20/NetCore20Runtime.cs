using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFX.PAL;

namespace NFX.PAL.NetCore20
{
  /// <summary>
  /// Represents .NET Framework runtime
  /// </summary>
  public static class NetCore20Runtime
  {
    /// <summary>
    /// Binds the NET Core 2.0 implementation to Platform Abstraction Layer
    /// This method must be called only once at the assembly entry point
    /// </summary>
    public static void Init()
    {
      IPALImaging imaging = null;

      IPALMachineInfo machine = null;
      IPALFileSystem fs = null;


      NFX.PAL.PlatformAbstractionLayer.____SetImplementation(
                   "Net Core 20",
                   imaging,
                   machine,
                   fs);
    }

    //https://stackoverflow.com/questions/3769405/determining-cpu-utilization

  }
}
