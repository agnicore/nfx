using System;
using System.Collections.Generic;
using System.Text;

using NFX.ServiceModel;

namespace NFX.Wave.Server
{
  /// <summary>
  /// Provides base abstraction for web servers
  /// </summary>
  public abstract class WebListener : Service<WaveServer>
  {
    protected WebListener(WaveServer director) : base(director)
    {

    }

    protected override void Destructor()
    {
      base.Destructor();
    }


  }
}
