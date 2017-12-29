using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using NFX.Graphics;
using NFX.PAL.Graphics;

namespace NFX.PAL.NetFramework.Graphics
{
  public sealed class NetImaging : IPALGraphics
  {
    public IPALImage CreateImage(string fileName)
    {
      throw new NotImplementedException();
    }

    public IPALImage CreateImage(byte[] data)
    {
      throw new NotImplementedException();
    }

    public IPALImage CreateImage(ArraySegment<byte> data)
    {
      throw new NotImplementedException();
    }

    public IPALImage CreateImage(Stream stream)
    {
      throw new NotImplementedException();
    }

    public IPALImage CreateImage(Size size, Size resolution, PixelFormat pixFormat)
    {
      return new NetImage(size, resolution, pixFormat);
    }
  }
}
