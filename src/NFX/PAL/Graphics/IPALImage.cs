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
    Color GetAveragePixel(Point p, Size area);
    void SetPixel(Point p, Color color);
    void SetPixel(PointF p, Color color);

    void MakeTransparent(Color? dflt);
    void Save(string fileName, ImageFormat format);
    byte[] Save(ImageFormat format);
    void Save(Stream stream, ImageFormat format);

  }
}
