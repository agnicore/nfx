using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using NFX.PAL;
using NFX.PAL.Graphics;

namespace NFX.Graphics
{
  /// <summary>
  /// Represents a 2d graphical in-memory Image.
  /// The purpose of this object is to provide basic image processing capabilities cross-platform.
  /// Graphics objects are NOT thread-safe
  /// </summary>
  public sealed class Image : DisposableObject
  {
    public const int DEFAULT_RESOLUTION_PPI = 72;

    /// <summary>Creates a new image instance from a named image file</summary>
    public static Image FromFile(string fileName) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(fileName));

    /// <summary>Creates a new image instance from image content contained in a byte[]</summary>
    public static Image FromBytes(byte[] data)    => new Image(PlatformAbstractionLayer.Graphics.CreateImage(data));

    /// <summary>Creates a new image instance from image content contained in a stream</summary>
    public static Image FromStream(Stream stream) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(stream));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(int width, int height) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    new Size(width, height),
                                                                    new Size(DEFAULT_RESOLUTION_PPI, DEFAULT_RESOLUTION_PPI),
                                                                    ImagePixelFormat.Default));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(int width, int height, int xDPI, int yDPI) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    new Size(width, height),
                                                                    new Size(xDPI, yDPI),
                                                                    ImagePixelFormat.Default));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(int width, int height, ImagePixelFormat pixFormat) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    new Size(width, height),
                                                                    new Size(DEFAULT_RESOLUTION_PPI, DEFAULT_RESOLUTION_PPI),
                                                                    pixFormat));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(int width, int height, int xDPI, int yDPI, ImagePixelFormat pixFormat) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    new Size(width, height),
                                                                    new Size(xDPI, yDPI),
                                                                    pixFormat));


    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(Size size) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    size,
                                                                    new Size(DEFAULT_RESOLUTION_PPI, DEFAULT_RESOLUTION_PPI),
                                                                    ImagePixelFormat.Default));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(Size size, ImagePixelFormat pixFormat) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(
                                                                    size,
                                                                    new Size(DEFAULT_RESOLUTION_PPI, DEFAULT_RESOLUTION_PPI),
                                                                    pixFormat));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(Size size, Size resolution) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(size, resolution, ImagePixelFormat.Default));

    /// <summary>Creates a new image instance of the specified properties</summary>
    public static Image Of(Size size, Size resolution, ImagePixelFormat format) => new Image(PlatformAbstractionLayer.Graphics.CreateImage(size, resolution, format));



    private Image(IPALImage handle) { m_Handle = handle; }

    protected override void Destructor()
    {
      base.Destructor();
      DisposeAndNull(ref m_Handle);
    }

    private IPALImage m_Handle;


    public Size Size { get{  EnsureObjectNotDisposed(); return m_Handle.GetSize(); } }
    public int Width  => Size.Width;
    public int Height => Size.Height;

    public Size Resolution { get{  EnsureObjectNotDisposed(); return m_Handle.GetResolution(); } }
    public int XResolution => Size.Width;
    public int YResolution => Size.Height;


    public void SetResolution(int xDPI, int yDPI) => SetResolution(new Size(xDPI, yDPI));

    public void SetResolution(Size resolution)
    {
      EnsureObjectNotDisposed();
      m_Handle.SetResolution(resolution);
    }

    public void MakeTransparent(Color? dflt = null)
    {
      EnsureObjectNotDisposed();
      m_Handle.MakeTransparent(dflt);
    }

    //todo: Lock bits

    public Color GetPixel(int x, int y) => GetPixel(new Point(x, y));
    public Color GetPixel(Point p)
    {
      EnsureObjectNotDisposed();
      return m_Handle.GetPixel(p);
    }

    public Color GetPixel(float x, float y) => GetPixel(new PointF(x, y));
    public Color GetPixel(PointF p)
    {
      EnsureObjectNotDisposed();
      return m_Handle.GetPixel(p);
    }

    /// <summary>
    /// Gets an average color of the specified pixel performing RGBA component average
    /// </summary>
    public Color GetAveragePixel(Point p, Size area)
    {
      EnsureObjectNotDisposed();
      return m_Handle.GetAveragePixel(p, area);
    }

    public void SetPixel(int x, int y, Color color) => SetPixel(new Point(x, y), color);
    public void SetPixel(Point p, Color color)
    {
      EnsureObjectNotDisposed();
      m_Handle.SetPixel(p, color);
    }

    public void SetPixel(float x, float y, Color color) => SetPixel(new PointF(x, y), color);
    public void SetPixel(PointF p, Color color)
    {
      EnsureObjectNotDisposed();
      m_Handle.SetPixel(p, color);
    }

    /// <summary>
    /// Saves image to the named file
    /// </summary>
    public void Save(string fileName, ImageFormat format)
    {
      EnsureObjectNotDisposed();
      m_Handle.Save(fileName, format);
    }

    /// <summary>
    /// Saves image to stream
    /// </summary>
    public void Save(Stream stream, ImageFormat format)
    {
      EnsureObjectNotDisposed();
      m_Handle.Save(stream, format);
    }

    /// <summary>
    /// Saves image to byte[]
    /// </summary>
    public byte[] Save(ImageFormat format)
    {
      EnsureObjectNotDisposed();
      return m_Handle.Save(format);
    }

  }
}
