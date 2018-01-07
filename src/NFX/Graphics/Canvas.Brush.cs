using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    public sealed class Brush : Asset<IPALCanvasBrush>
    {
      internal Brush(IPALCanvasBrush handle) : base(handle)
      {

      }

      public Color Color => Handle.Color;
    }
  }
}
