using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NFX.PAL;
using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  /// <summary>
  /// Represents a 2d drawing surface
  /// </summary>
  public sealed partial class Canvas : DisposableObject
  {
    /// <summary>
    /// Returns true when all assets (such as brushes, pens, fonts) are owned by the canvas instance and
    /// they can not be used/persisted beyond the scope of this canvas lifecycle.
    /// Some libraries allow to create objects and cache them for subsequent use, whereas others (e.g. Windows classic GDI)
    /// mandate that all graphics handles belong to the particular canvas instance and get invalidated when that instance
    /// gets releases via Dispose()
    /// </summary>
    public static bool OwnsAssets =>  PlatformAbstractionLayer.Graphics.CanvasOwnsAssets;


    internal Canvas(IPALCanvas handle){ m_Handle = handle;}

    protected override void Destructor()
    {
      base.Destructor();
      DisposeAndNull(ref m_Handle);
    }

    private IPALCanvas m_Handle;

    /// <summary>
    /// Defines how pixel colors are approximated/interpolated during image resize, rotate and other operations
    /// which distort 1:1 pixel mappings
    /// </summary>
    public InterpolationMode Interpolation
    {
      get { EnsureObjectNotDisposed(); return m_Handle.Interpolation;  }
      set { EnsureObjectNotDisposed(); m_Handle.Interpolation = value; }
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

    public Pen CreateSolidPen(Color color, float width)
    {
      return null;
    }

    public Brush CreateSolidBrush(Color color)
    {
      return null;
    }

  }
}
