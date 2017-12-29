using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NBmp=System.Drawing.Bitmap;

using NFX.Graphics;
using NFX.PAL.Graphics;

namespace NFX.PAL.NetFramework.Graphics
{
  /// <summary>
  /// Implements image using .NET framework GDI+ wrapper
  /// </summary>
  public sealed class NetImage : DisposableObject, IPALImage
  {
    public static System.Drawing.Imaging.PixelFormat ToGDIPixelFormat(PixelFormat pf)
    {
      switch(pf)
      {
        case PixelFormat.BPP1Indexed: return System.Drawing.Imaging.PixelFormat.Format1bppIndexed;
        case PixelFormat.BPP4Indexed: return System.Drawing.Imaging.PixelFormat.Format4bppIndexed;
        case PixelFormat.BPP8Indexed: return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
        case PixelFormat.BPP16Gray: return System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
        case PixelFormat.RGB24: return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
        case PixelFormat.RGB32: return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
        case PixelFormat.RGBA32: return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        default: return System.Drawing.Imaging.PixelFormat.Canonical;
      }
    }

    internal NetImage(Size size, Size resolution, PixelFormat pixFormat)
    {
      var pf = ToGDIPixelFormat(pixFormat);
      m_Bitmap = new NBmp(size.Width, size.Height, pf);
      m_Bitmap.SetResolution(resolution.Width, resolution.Height);
    }

    protected override void Destructor()
    {
      base.Destructor();
      DisposeAndNull(ref m_Bitmap);
    }

    private NBmp m_Bitmap;

    public Color GetAveragePixel(Point p, Size area)
    {
      #warning Implement!!!!
      return Color.White;
    }

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

    public void Save(Stream stream, NFX.Graphics.ImageFormat format)
    {
      throw new NotImplementedException();
    }

  }
}
