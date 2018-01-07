using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    public sealed class Pen : Asset<IPALCanvasPen>
    {
      internal Pen(IPALCanvasPen handle):base(handle)
      {

      }

      public Color Color => Handle.Color;
      public float Width => Handle.Width;
      public PenDashStyle DashStyle => Handle.DashStyle;
    }

  }
}
