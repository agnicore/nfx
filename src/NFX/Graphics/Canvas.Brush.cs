using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NFX.Graphics
{
  public sealed partial class Canvas : DisposableObject
  {
    public abstract class Brush : Object
    {

    }

    public sealed class SolidBrush : Brush
    {
      public SolidBrush(Color color){ Color = color;}
      public readonly Color Color;
    }
  }
}
