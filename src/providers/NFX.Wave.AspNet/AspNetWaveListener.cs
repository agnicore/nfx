using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Http;

using NFX;
using NFX.Wave;
using NFX.Wave.Server;

namespace NFX.Wave.AspNet
{
  /// <summary>
  /// Implements WaveListener based on AspNet middleware
  /// </summary>
  public sealed class AspNetWaveListener : WaveListener
  {
    public AspNetWaveListener(WaveServer waveServer) : base(waveServer)
    {

    }


  }
}
