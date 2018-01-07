using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

using NFX.Graphics;
using NFXImageFormat=NFX.Graphics.ImageFormat;
using NFX.PAL.Graphics;

namespace NFX.PAL.NetFramework.Graphics
{
  /// <summary>
  /// Implements image using .NET framework GDI+ wrapper
  /// </summary>
  public sealed class NetImage : DisposableObject, IPALImage
  {

    internal NetImage(Size size, Size resolution, ImagePixelFormat pixFormat)
    {
      var pf = xlator.xlat(pixFormat);
      m_Bitmap = new Bitmap(size.Width, size.Height, pf);
      m_Bitmap.SetResolution(resolution.Width, resolution.Height);
    }

    internal NetImage(System.Drawing.Image img)
    {
      var bmp = img as Bitmap;
      if (bmp==null)
        throw new NetFrameworkPALException(StringConsts.ARGUMENT_ERROR + $"{nameof(NetImage)}.ctor(img!bitmap)");
      m_Bitmap = bmp;
    }

    protected override void Destructor()
    {
      base.Destructor();
      DisposeAndNull(ref m_Bitmap);
    }

    private Bitmap m_Bitmap;

    internal Bitmap Bitmap => m_Bitmap;

    public ImagePixelFormat PixelFormat => xlator.xlat(m_Bitmap.PixelFormat);

    public Color GetPixel(Point p) => m_Bitmap.GetPixel(p.X, p.Y);
    public void SetPixel(Point p, Color color) => m_Bitmap.SetPixel(p.X, p.Y, color);

    public Color GetPixel(PointF p) => m_Bitmap.GetPixel((int)p.X, (int)p.Y);
    public void SetPixel(PointF p, Color color) => m_Bitmap.SetPixel((int)p.X, (int)p.Y, color);

    public Size GetResolution() => new Size((int)m_Bitmap.HorizontalResolution, (int)m_Bitmap.VerticalResolution);
    public void SetResolution(Size resolution) => m_Bitmap.SetResolution(resolution.Width, resolution.Height);

    public Size GetSize() => m_Bitmap.Size;


    public void MakeTransparent(Color? dflt)
    {
      if (dflt.HasValue)
        m_Bitmap.MakeTransparent(dflt.Value);
      else
        m_Bitmap.MakeTransparent();
    }

    public IPALCanvas CreateCanvas()
    {
      var ngr = System.Drawing.Graphics.FromImage(m_Bitmap);
      return new NetCanvas(ngr);
    }

    public void Save(string fileName, NFXImageFormat format)
    {
      var (codec, pars) = getEncoder(format);
      m_Bitmap.Save(fileName, codec, pars);
    }

    public void Save(Stream stream, NFXImageFormat format)
    {
      var (codec, pars) = getEncoder(format);
      m_Bitmap.Save(stream, codec, pars);
    }

    public byte[] Save(NFXImageFormat format)
    {
      using(var ms = new MemoryStream())
      {
        this.Save(ms, format);
        return ms.ToArray();
      }
    }

    private (ImageCodecInfo codec, EncoderParameters pars) getEncoder(NFXImageFormat format)
    {
      ImageCodecInfo codec;
      EncoderParameters pars;

      if (format is BitmapImageFormat)
      {
        codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Bmp.Guid);
        pars = new EncoderParameters(1);
        pars.Param[0] = new EncoderParameter(Encoder.ColorDepth, format.Colors);
      } else if (format is PngImageFormat)
      {
        codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Png.Guid);
        pars = null;//new EncoderParameters(0);
      } else if (format is GifImageFormat)
      {
        codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Gif.Guid);
        pars = null;//new EncoderParameters(0);
      } else//default is JPEG
      {
        var jpeg = format as JpegImageFormat;
        codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
        pars = new EncoderParameters(1);
        pars.Param[0] = new EncoderParameter(Encoder.Quality, jpeg?.Quality ?? 80L);
      }


      return ( codec: codec, pars: null );
    }
  }
}
