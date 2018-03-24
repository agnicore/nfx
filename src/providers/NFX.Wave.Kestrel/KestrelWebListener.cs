using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Server.Kestrel.Core;

using NFX;
using NFX.Wave;
using NFX.Wave.Server;

namespace NFX.Wave.Kestrel
{
  public sealed class KestrelWebListener : WebListener
  {
    public KestrelWebListener(WaveServer waveServer) : base(waveServer)
    {
       m_Kestrel = new KestrelServer(null, null, null);
    }

    private KestrelServer m_Kestrel;

  }
}
