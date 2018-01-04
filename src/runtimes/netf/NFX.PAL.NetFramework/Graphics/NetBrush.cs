using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

using NFX.Graphics;
using NFXImageFormat=NFX.Graphics.ImageFormat;
using NFX.PAL.Graphics;

namespace NFX.PAL.NetFramework.Graphics
{
  /// <summary>
  /// Implements brush using .NET framework GDI+ wrapper
  /// </summary>
  public sealed class NetBrush : DisposableObject, IPALCanvasBrush
  {
    internal NetBrush(Color color)
    {
      m_Brush = new SolidBrush(color);
    }

    protected override void Destructor()
    {
      base.Destructor();
      DisposeAndNull(ref m_Brush);
    }

    private Brush m_Brush;

    internal Brush GDIBrush => m_Brush;
  }
}
