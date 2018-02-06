using System;
using System.IO;
using System.Drawing;

using NFX.Graphics;

namespace NFX.PAL.Graphics
{
  public interface IPALImage : IDisposable
  {
    Size GetSize();

    ImagePixelFormat PixelFormat { get; }
    ImageFormat LoadFormat{ get; }

    /// <summary>
    /// PPI resolution
    /// </summary>
    int GetXResolution();
    int GetYResolution();
    void SetResolution(int xDPI, int yDPI);

    Color GetPixel(Point p);
    Color GetPixel(PointF p);
    void SetPixel(Point p, Color color);
    void SetPixel(PointF p, Color color);

    void MakeTransparent(Color? dflt);
    void Save(string fileName, ImageFormat format);
    byte[] Save(ImageFormat format);
    void Save(Stream stream, ImageFormat format);

    IPALCanvas CreateCanvas();

  }
}
