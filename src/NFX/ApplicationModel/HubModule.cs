using NFX.Environment;

namespace NFX.ApplicationModel
{
  /// <summary>
  /// Defines a module that does nothing else but provides a hub/namespace grouping for child modules that it contains.
  /// This module is a kin to NOPModule - the difference is only in the intent. NOPModule signifies the absence of any modules,
  /// whereas HubModule holds child modules
  /// </summary>
  public sealed class HubModule : ModuleBase
  {
    public HubModule() : base(){ }
    public HubModule(IModule parent) : base(parent){ }
    public HubModule(IModule parent, int order) : base(parent, order){ }
    public override bool IsHardcodedModule => false;
  }
}
