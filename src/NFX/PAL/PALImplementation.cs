using System;
using System.Collections.Generic;
using System.Text;

using NFX.ApplicationModel;

namespace NFX.PAL
{
  /// <summary>
  /// Provides base for all platform abstraction layer implementations
  /// </summary>
  public abstract class PALImplementation : ApplicationComponent, INamed
  {
    protected PALImplementation() : base() { }


    public abstract string Name { get; }
    public abstract Graphics.IPALGraphics Graphics     { get; }
    public abstract IPALMachineInfo       MachineInfo  { get; }
    public abstract IPALFileSystem        FileSystem   { get; }
  }
}
