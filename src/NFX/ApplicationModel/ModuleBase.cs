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
  /// Provides base for implementation of IModule
  /// </summary>
  public abstract class ModuleBase : ApplicationComponent, IModuleImplementation
  {
    /// <summary>
    /// Creates a root module without a parent
    /// </summary>
    protected ModuleBase() : base(){ }

    protected ModuleBase(ModuleBase parent) : base(parent) { }

    protected override void Destructor()
    {
      cleanupChildren(true);
      base.Destructor();
    }

    private void cleanupChildren(bool all)
    {
      var toClean = m_Children.Where(c => c.ParentModule==this && (all || !c.IsHardcodedModule)).ToList();
      toClean.ForEach( c =>
                       {
                         c.Dispose();
                         m_Children.Unregister(c);
                       });
    }

    [Config] private string m_Name;
    [Config] private int m_Order;

    protected OrderedRegistry<ModuleBase> m_Children = new OrderedRegistry<ModuleBase>();

    public MessageType? ModuleLogLevel { get; set; }

    public virtual MessageType ModuleEffectiveLogLevel
    {
      get
      {
        return ModuleLogLevel ?? ParentModule?.ModuleEffectiveLogLevel ?? MessageType.Info;
      }
    }

    public IModule ParentModule { get{ return ComponentDirector as IModule;} }

    public IOrderedRegistry<IModule> ChildModules { get{ return m_Children;} }

    public string Name { get{ return m_Name;} }

    public int Order { get{ return m_Order;} }

    public virtual bool InstrumentationEnabled { get; set; }

    public abstract bool IsHardcodedModule{ get; }


    void IConfigurable.Configure(IConfigSectionNode node)
    {
      ConfigAttribute.Apply(this, node);
      if (m_Name.IsNullOrWhiteSpace()) m_Name = this.GetType().Name;
      DoConfigureChildModules(node);
      DoConfigure(node);
    }

    IEnumerable<KeyValuePair<string, Type>> IExternallyParameterized.ExternalParameters
    {
      get { return DoGetExternalParameters(); }
    }

    bool IExternallyParameterized.ExternalGetParameter(string name, out object value, params string[] groups)
    {
      return DoExternalGetParameter(name, out value, groups);
    }

    IEnumerable<KeyValuePair<string, Type>> IExternallyParameterized.ExternalParametersForGroups(params string[] groups)
    {
      return DoGetExternalParametersForGroups(groups);
    }

    bool IExternallyParameterized.ExternalSetParameter(string name, object value, params string[] groups)
    {
      return DoExternalSetParameter(name, value, groups);
    }

    public virtual Guid ModuleLog(MessageType type,
                                  string from,
                                  string text,
                                  Exception error = null,
                                  Guid? related = null,
                                  string pars = null)
    {
      if (type<ModuleEffectiveLogLevel) return Guid.Empty;

      var msg = new Message
      {
        Topic = CoreConsts.APP_MODULE_TOPIC,
        From = "{0}.{1}".Args(this, from),
        Type = type,
        Text = text,
        Exception = error,
        Parameters = pars,
      };

      if (related.HasValue) msg.RelatedTo = related.Value;

      App.Log.Write( msg );

      return msg.Guid;
    }

    public override string ToString()
    {
      return "Module {0}('{1}', {2})".Args(GetType().DisplayNameWithExpandedGenericArgs(), Name, Order);
    }

    /// <summary> Override to configure the instance </summary>
    protected virtual void DoConfigure(IConfigSectionNode node) { }

    /// <summary>
    /// Override to perform custom population/registration of modules
    /// </summary>
    protected virtual void DoConfigureChildModules(IConfigSectionNode node)
    {
      cleanupChildren(false);
      if (node==null || !node.Exists) return;

      var allModules = DoGetAllChildModuleConfigNodes(node);
      foreach(var mnode in allModules)
      {
        var module = FactoryUtils.MakeAndConfigure<ModuleBase>(mnode, null, new []{this});
        if (!m_Children.Register(module))
         throw new NFXException(StringConsts.APP_MODULE_DUPLICATE_CHILD_ERROR.Args(this, module));
      }
    }

    protected virtual IEnumerable<IConfigSectionNode> DoGetAllChildModuleConfigNodes(IConfigSectionNode node)
    {
      if (node==null || !node.Exists) return Enumerable.Empty<IConfigSectionNode>();

      node = node[CommonApplicationLogic.CONFIG_MODULES_SECTION];

      return node.Children.Where(c => c.IsSameName(CommonApplicationLogic.CONFIG_MODULE_SECTION));
    }

    protected virtual IEnumerable<KeyValuePair<string, Type>> DoGetExternalParameters()
    {
      return ExternalParameterAttribute.GetParameters(this);
    }


    protected virtual bool DoExternalGetParameter(string name, out object value, params string[] groups)
    {
      return ExternalParameterAttribute.GetParameter(this, name, out value, groups);
    }

    protected virtual bool DoExternalSetParameter(string name, object value, params string[] groups)
    {
      return ExternalParameterAttribute.SetParameter(this, name, value, groups);
    }

    protected virtual IEnumerable<KeyValuePair<string, Type>> DoGetExternalParametersForGroups(params string[] groups)
    {
      return ExternalParameterAttribute.GetParameters(this, groups);
    }

  }
}
