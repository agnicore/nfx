/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2018 Agnicore Inc. portions ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/


using System;
using System.Drawing;
using NFX.Graphics;
using NFX.Scripting;

namespace NFX.UTest.Graphics
{
  [Runnable(TRUN.BASE)]
  public class BaseImageTests : GraphicsTestBase
  {
    [Run]
    public void Image_Ctors()
    {
      using (var img = Image.Of(23, 56)) // ImageFormmat must be Format32bppArgb.   otherwise AgrumentException
      {
        Aver.AreEqual(23, img.Width);
        Aver.AreEqual(56, img.Height);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.XResolution);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(45, 57, 300, 400))
      {
        Aver.AreEqual(45, img.Width);
        Aver.AreEqual(57, img.Height);
        Aver.AreEqual(300, img.XResolution);
        Aver.AreEqual(400, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(12, 45, ImagePixelFormat.BPP16Gray))
      {
        Aver.AreEqual(12, img.Width);
        Aver.AreEqual(45, img.Height);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.XResolution);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(45, 57, 200, 300, ImagePixelFormat.BPP1Indexed))
      {
        Aver.AreEqual(45, img.Width);
        Aver.AreEqual(57, img.Height);
        Aver.AreEqual(200, img.XResolution);
        Aver.AreEqual(300, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(new Size(23, 56)))
      {
        Aver.AreEqual(23, img.Width);
        Aver.AreEqual(56, img.Height);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.XResolution);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(new Size(12, 45), ImagePixelFormat.BPP16Gray))
      {
        Aver.AreEqual(12, img.Width);
        Aver.AreEqual(45, img.Height);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.XResolution);
        Aver.AreEqual(Image.DEFAULT_RESOLUTION_PPI, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(new Size(45, 57), new Size(300, 400)))
      {
        Aver.AreEqual(45, img.Width);
        Aver.AreEqual(57, img.Height);
        Aver.AreEqual(300, img.XResolution);
        Aver.AreEqual(400, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }

      using (var img = Image.Of(new Size(45, 57), new Size(200, 300), ImagePixelFormat.BPP1Indexed))
      {
        Aver.AreEqual(45, img.Width);
        Aver.AreEqual(57, img.Height);
        Aver.AreEqual(200, img.XResolution);
        Aver.AreEqual(300, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);
      }
    }

    [Run]
    public void Image_GetSize()
    {
      using (var img = Image.Of(23, 56))
      {
        Aver.AreEqual(23, img.Size.Width);
        Aver.AreEqual(23, img.Width);
        Aver.AreEqual(56, img.Size.Height);
        Aver.AreEqual(56, img.Height);
      }
    }

    [Run]
    public void Image_GetSetResolution()
    {
      using (var img = Image.Of(23, 56, 72, 300))
      {
        Aver.AreEqual(72,  img.XResolution);
        Aver.AreEqual(300, img.YResolution);

        img.SetResolution(200, 400);
        Aver.AreEqual(200, img.XResolution);
        Aver.AreEqual(400, img.YResolution);
      }
    }

    [Run]
    public void Image_GetSetPixel()
    {
      using (var img = Image.Of(23, 56))
      {
        var c = Color.FromArgb(20, 34, 56, 120);
        img.SetPixel(2, 5, c);
        var c1 = img.GetPixel(2, 5);
        var c2 = img.GetPixel(new Point(2, 5));
        var c3 = img.GetPixel(2.1F, 5.3F);
        var c4 = img.GetPixel(new PointF(2.1F, 5.3F));

        Aver.AreObjectsEqual(c, c1);
        Aver.AreObjectsEqual(c, c2);
        Aver.AreObjectsEqual(c, c3);
        Aver.AreObjectsEqual(c, c4);

        img.SetPixel(new Point(4, 5), c);
        c1 = img.GetPixel(4, 5);
        c2 = img.GetPixel(new Point(4, 5));
        c3 = img.GetPixel(4.1F, 5.3F);
        c4 = img.GetPixel(new PointF(4.1F, 5.3F));


        Aver.AreObjectsEqual(c, c1);
        Aver.AreObjectsEqual(c, c2);
        Aver.AreObjectsEqual(c, c3);
        Aver.AreObjectsEqual(c, c4);

        img.SetPixel(2.1F, 5.7F, c);
        c1 = img.GetPixel(2, 5);
        c2 = img.GetPixel(new Point(2, 5));
        c3 = img.GetPixel(2.1F, 5.3F);
        c4 = img.GetPixel(new PointF(2.1F, 5.3F));

        Aver.AreObjectsEqual(c, c1);
        Aver.AreObjectsEqual(c, c2);
        Aver.AreObjectsEqual(c, c3);
        Aver.AreObjectsEqual(c, c4);

        img.SetPixel(new PointF(4.5F, 5.1F), c);
        c1 = img.GetPixel(4, 5);
        c2 = img.GetPixel(new Point(4, 5));
        c3 = img.GetPixel(4.1F, 5.3F);
        c4 = img.GetPixel(new PointF(4.1F, 5.3F));

        Aver.AreObjectsEqual(c, c1);
        Aver.AreObjectsEqual(c, c2);
        Aver.AreObjectsEqual(c, c3);
        Aver.AreObjectsEqual(c, c4);
      }
    }

    [Run]
    public void Image_ResizeTo()
    {
      using (var img = TestImg1.ResizeTo(7, 3))
      {
        Aver.AreEqual(7,  img.Width);
        Aver.AreEqual(3,  img.Height);
        Aver.AreEqual(72, img.XResolution);
        Aver.AreEqual(72, img.YResolution);
        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);

        var p00 = img.GetPixel(0, 0);
        var p01 = img.GetPixel(0, 1);
        var p10 = img.GetPixel(1, 0);
        var p11 = img.GetPixel(1, 1);

        Aver.AreObjectsEqual(Color.FromArgb(255,  0, 255, 0),   p00);
        Aver.AreObjectsEqual(Color.FromArgb(255,  0, 255, 0),   p01);
        Aver.AreObjectsEqual(Color.FromArgb(255, 36,   0, 219), p10);
        Aver.AreObjectsEqual(Color.FromArgb(255, 12,   0, 243), p11);
      }
    }
  }
}
