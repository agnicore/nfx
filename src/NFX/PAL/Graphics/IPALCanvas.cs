using System;
using System.IO;
using System.Drawing;

using NFX.Graphics;

namespace NFX.PAL.Graphics
{

  public interface IPALCanvasAsset : IDisposable { }

  public interface IPALCanvasBrush : IPALCanvasAsset { }
  public interface IPALCanvasPen   : IPALCanvasAsset { }
  public interface IPALCanvasFont  : IPALCanvasAsset { }

  public interface IPALCanvas : IDisposable
  {
    InterpolationMode Interpolation{ get; set;}


    IPALCanvasBrush CreateSolidBrush(Color color);

    void Clear(Color color);
    void FillRectangle(IPALCanvasBrush brush, Rectangle rect);
    void FillRectangle(IPALCanvasBrush brush, RectangleF rect);

    void DrawImage(IPALImage image, Rectangle rect);
    void DrawImage(IPALImage image, RectangleF rect);
    void DrawImage(IPALImage image, Rectangle src, Rectangle dest);
    void DrawImage(IPALImage image, RectangleF src, RectangleF dest);
  }
}
