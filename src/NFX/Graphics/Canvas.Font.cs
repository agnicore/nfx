using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    public sealed class Font : Asset<IPALCanvasFont>
    {
      internal Font(IPALCanvasFont handle) : base(handle)
      {

      }
      public string Name => Handle.Name;
      public float Size => Handle.Size;
      public FontStyling Style => Handle.Style;
      public MeasureUnit Unit => Handle.Unit;
    }
  }
}
