using System;
using System.Collections.Generic;
using System.Text;

namespace NFX.Graphics
{
    public enum InterpolationMode
    {
      Default = 0,
      Bicubic,
      Bilinear,
      Low,
      High,
      NearestNeighbor,
      HQBilinear,
      HQBicubic
    }

    /// <summary>
    /// Specifies image pixel format
    /// </summary>
    public enum PixelFormat
    {
      /// <summary>
      /// The default pixel format of 32 bits per pixel. The format specifies 24-bit color depth and an 8-bit alpha channel
      /// </summary>
      Default = 0,

      /// <summary>Monochrome </summary>
      BPP1Indexed,

      /// <summary>4 bit palette </summary>
      BPP4Indexed,

      /// <summary>8 bit palette </summary>
      BPP8Indexed,

      /// <summary>
      /// 2^16 shades of gray
      /// </summary>
      BPP16Gray,

      /// <summary>8 bit per R G and B </summary>
      RGB24,

      /// <summary> R G B take 8 bits remaining 4th byte is unused</summary>
      RGB32,

      /// <summary> R G B take 8 bits remaining 4th byte is Alpha</summary>
      RGBA32
    }
}
