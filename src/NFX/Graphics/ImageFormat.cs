using System;
using System.Collections.Generic;
using System.Text;

namespace NFX.Graphics
{
  public abstract class ImageFormat
  {

    /// <summary>
    /// Gets bits per pixel
    /// </summary>
    public abstract int BPP{ get;}

    public abstract string WebContentType { get;}

  }


  public sealed class BitmapImageFormat : ImageFormat
  {
    public static readonly BitmapImageFormat Monochrome = new BitmapImageFormat(2);

    public BitmapImageFormat(int colors)
    {

    }


    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/bmp"; }
  }

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

  public sealed class GifImageFormat : ImageFormat
  {
    public static readonly GifImageFormat Monochrome = new GifImageFormat(2);

    public GifImageFormat(int colors)
    {

    }

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/gif"; }
  }

  public sealed class JpegImageFormat : ImageFormat
  {
    public static readonly JpegImageFormat Standard = new JpegImageFormat(80);

    public JpegImageFormat(int compression)
    {

    }

    public override int BPP{ get => 0;}
    public override string WebContentType { get => "image/jpeg"; }
  }
}
