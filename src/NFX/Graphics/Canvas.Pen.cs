using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    public abstract class Pen : Asset<IPALCanvasPen>
    {
      protected Pen(IPALCanvasPen handle):base(handle)
      {

      }
    }

  }
}
