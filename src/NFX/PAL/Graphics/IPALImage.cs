using System;
using System.IO;
using System.Drawing;

using NFX.Graphics;

namespace NFX.PAL.Graphics
{
  public interface IPALImage : IDisposable
  {
    Size GetSize();

    /// <summary>
    /// PPI resolution
    /// </summary>
    Size GetResolution();
    void SetResolution(Size resolution);

    Color GetPixel(Point p);
    Color GetPixel(PointF p);
    void SetPixel(Point p, Color color);
    void SetPixel(PointF p, Color color);

    void MakeTransparent();
    void Save(Stream stream, ImageFormat format);
  }
}
