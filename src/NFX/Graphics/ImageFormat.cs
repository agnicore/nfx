using System;
using System.Collections.Generic;
using System.Text;

namespace NFX.Graphics
{
  /// <summary>
  /// Base class for representing various formats of images, such as Hi-Quality Jpeg, or monochrome Png etc.
  /// </summary>
  public abstract class ImageFormat
  {

    /// <summary>
    /// Gets bits per pixel
    /// </summary>
    public abstract int BPP{ get;}

    public abstract string WebContentType { get;}

  }

  /// <summary>
  /// Represents Bitmap image format
  /// </summary>
  public sealed class BitmapImageFormat : ImageFormat
  {
    public static readonly BitmapImageFormat Monochrome = new BitmapImageFormat(2);

    public BitmapImageFormat(int colors)
    {

    }

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/bmp"; }
  }

  /// <summary>
  /// Represents Png image format
  /// </summary>
  public sealed class PngImageFormat : ImageFormat
  {
    public static readonly PngImageFormat Monochrome = new PngImageFormat(2);

    public static readonly PngImageFormat Standard = new PngImageFormat();


    public PngImageFormat()//create full resolution
    {
    }

    public PngImageFormat(int colors)
    {

    }

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/png"; }
  }

  /// <summary>
  /// Represents Gif image format
  /// </summary>
  public sealed class GifImageFormat : ImageFormat
  {
    public static readonly GifImageFormat Monochrome = new GifImageFormat(2);

    public GifImageFormat(int colors)
    {

    }

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/gif"; }
  }


  /// <summary>
  /// Represents Jpeg image format of the specified quality
  /// </summary>
  public sealed class JpegImageFormat : ImageFormat
  {
    /// <summary>
    /// Standard Jpeg compression of 80 quality
    /// </summary>
    public static readonly JpegImageFormat Standard = new JpegImageFormat(80);

    public JpegImageFormat(int compression)
    {
      if (compression<0 || compression>100)
        throw new GraphicsException(StringConsts.ARGUMENT_ERROR+"{0}.ctor(compression<0|>100)".Args(nameof(JpegImageFormat)));

      Compression = compression;
    }

    public readonly int Compression;

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/jpeg"; }
  }
}
