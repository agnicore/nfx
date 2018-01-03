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
      protected Font(IPALCanvasFont handle) : base(handle)
      {

      }
      public readonly string FamilyName;
    }
  }
}
