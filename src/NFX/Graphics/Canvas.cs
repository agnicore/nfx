using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NFX.Graphics
{
  /// <summary>
  /// Represents a 2d drawing surface
  /// </summary>
  public sealed partial class Canvas : DisposableObject
  {
    public Canvas(Image image)
    {

    }

    protected override void Destructor()
    {
      base.Destructor();
    }


    public InterpolationMode InterpolationMode
    {
      get;
      set;
    }



    public void DrawImage(Image image, Rectangle rect)
    {
    }

    public void DrawImage(Image image, float x, float y, float w, float h)
    {
    }



    public void DrawImage(Image image, Rectangle src, Rectangle dest)
    {
    }

    public void Clear(Color color)
    {

    }

    public void FillRectangle(Canvas.Brush brush, float x, float y, float w, float h)
    {

    }

  }
}
