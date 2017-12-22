using System;
using System.Collections.Generic;
using System.Text;

using NFX.Environment;
using NFX.Instrumentation;

namespace NFX.ApplicationModel
{
  /// <summary>
  /// Describes application modules - entities that contain business domain logic of the application or
  /// general system logic (e.g. social network mix-in)
  /// </summary>
  public interface IModule : IApplicationComponent, INamed, IOrdered
  {
    /// <summary>
    /// References a parent logic module, or null if this is a root module injected in the application container
    /// </summary>
    IModule ParentModule { get; }

    /// <summary>
    /// Returns true when the module is injected in the parent context by the code, not configuration
    /// </summary>
    bool IsHardcodedModule { get; }

    /// <summary>
    /// Enumerates an ordered collection of child modules and provides access by name
    /// </summary>
    IOrderedRegistry<IModule> ChildModules { get; }

    /// <summary>
    /// Determines the effective log level for this module, taking it from parent if it is not defined on this level
    /// </summary>
    NFX.Log.MessageType ModuleEffectiveLogLevel { get; }
  }

  /// <summary>
  /// Describes module implementation
  /// </summary>
  public interface IModuleImplementation : IModule, IDisposable, IConfigurable, IInstrumentable
  {
    /// <summary>
    /// Defines log level for this module, if not defined then the component logger uses the parent log level
    /// via the ModuleEffectiveLogLevel property
    /// </summary>
    NFX.Log.MessageType? ModuleLogLevel { get; set; }

    /// <summary>
    /// Writes a log message through logic module; returns the new log msg GDID for correlation, or GDID.Empty if no message was logged
    /// </summary>
    Guid ModuleLog(NFX.Log.MessageType type, string from, string text, Exception error = null, Guid? related = null, string pars = null);
  }
}
