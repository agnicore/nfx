using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NFX.Environment;
using NFX.Instrumentation;
using NFX.Log;

namespace NFX.ApplicationModel
{
  /// <summary>
  /// Defines a module that does nothing
  /// </summary>
  public sealed class NOPModule : ModuleBase
  {
    private static NOPModule s_Instance = new NOPModule();

    private NOPModule() : base(){ }

    /// <summary>
    /// Returns a singleton instance of the NOPModule
    /// </summary>
    public static NOPModule Instance
    {
      get { return s_Instance; }
    }

    protected override void DoConfigureChildModules(IConfigSectionNode node)
    {
    }
  }
}
