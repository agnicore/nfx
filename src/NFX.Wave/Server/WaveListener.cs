using System;
using System.Collections.Generic;
using System.Text;

using NFX.ServiceModel;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Provides base abstraction for web server listeners
  /// </summary>
  public abstract class WaveListener : Service<WaveServer>
  {
    protected WaveListener(WaveServer director) : base(director)
    {

    }

    protected override void Destructor()
    {
      base.Destructor();
    }


  }
}
