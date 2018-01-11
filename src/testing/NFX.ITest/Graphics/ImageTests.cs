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
using System.IO;
using System.Reflection;
using NFX.Graphics;
using NFX.Scripting;

namespace NFX.ITest.Graphics
{
  [Runnable]
  public class ImageTests : IRunHook
  {
    #region CONSTS

      public const string TEST_JPG_FILENAME = @"Graphics/test.jpg";
      public const string TEST_PNG_FILENAME = @"Graphics/test.png";
      public const string STUB_JPG_FILENAME = @"Graphics/__saved.jpg";
      public const string STUB_PNG_FILENAME = @"Graphics/__saved.png";

    #endregion

    #region Init/TearDown

      bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
      {
        if (File.Exists(STUB_JPG_FILENAME)) File.Delete(STUB_JPG_FILENAME);
        if (File.Exists(STUB_PNG_FILENAME)) File.Delete(STUB_PNG_FILENAME);
        return false; //<--- The exception is NOT handled here, do default handling
      }

      bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
      {
        if (File.Exists(STUB_JPG_FILENAME)) File.Delete(STUB_JPG_FILENAME);
        if (File.Exists(STUB_PNG_FILENAME)) File.Delete(STUB_PNG_FILENAME);
        return false; //<--- The exception is NOT handled here, do default handling
      }

    #endregion

    [Run]
    public void Image_FromFile()
    {
      using (var img = Image.FromFile(TEST_PNG_FILENAME))
      {
        averTestImage(img);
      }
    }

    [Run]
    public void Image_FromStream()
    {
      using (var file = File.OpenRead(TEST_PNG_FILENAME))
      using (var img = Image.FromStream(file))
      {
        averTestImage(img);
      }
    }

    [Run]
    public void Image_FromBytes()
    {
      using (var file = File.OpenRead(TEST_PNG_FILENAME))
      using (var ms = new MemoryStream())
      {
        file.CopyTo(ms);

        using (var img = Image.FromBytes(ms.ToArray()))
        {
          averTestImage(img);
        }
      }
    }

    [Run]
    public void Image_SaveToFile()
    {
      using (var imgSrc = Image.FromFile(TEST_PNG_FILENAME))
      {
        imgSrc.Save(STUB_PNG_FILENAME, new PngImageFormat());

        using (var imgTrg = Image.FromFile(STUB_PNG_FILENAME))
        {
          averTestImage(imgTrg);
        }
      }
    }

    [Run]
    public void Image_SaveToStream()
    {
      using (var ms = new MemoryStream())
      using (var imgSrc = Image.FromFile(TEST_PNG_FILENAME))
      {
        imgSrc.Save(ms, new PngImageFormat(16));

        using (var imgTrg = Image.FromStream(ms))
        {
          averTestImage(imgTrg);
        }
      }
    }

    [Run]
    public void Image_SaveToBytes()
    {
      using (var ms = new MemoryStream())
      using (var imgSrc = Image.FromFile(TEST_PNG_FILENAME))
      {
        imgSrc.Save(ms, new PngImageFormat(16));

        using (var imgTrg = Image.FromBytes(ms.ToArray()))
        {
          averTestImage(imgTrg);
        }
      }
    }

    #region .pvt

      private void averTestImage(Image img)
      {
        //[ g, b, r, b, b, r, b, g ]
        //[ g, b, b, b, b, b, b, g ]
        //[ g, b, r, b, b, r, b, g ]
        //[ g, g, b, r, r, b, g, g ]

        var r = Color.FromArgb(255, 0, 0);
        var g = Color.FromArgb(0, 255, 0);
        var b = Color.FromArgb(0, 0, 255);

        Aver.AreEqual(8, img.Width);
        Aver.AreEqual(4, img.Height);
        Aver.AreEqual(119, img.XResolution);
        Aver.AreEqual(119, img.YResolution);

        Aver.AreObjectsEqual(ImagePixelFormat.RGBA32, img.PixelFormat);

        Aver.AreObjectsEqual(g, img.GetPixel(0, 0));
        Aver.AreObjectsEqual(b, img.GetPixel(1, 0));
        Aver.AreObjectsEqual(r, img.GetPixel(2, 0));
        Aver.AreObjectsEqual(b, img.GetPixel(3, 0));
        Aver.AreObjectsEqual(g, img.GetPixel(7, 0));
        Aver.AreObjectsEqual(g, img.GetPixel(0, 1));
        Aver.AreObjectsEqual(b, img.GetPixel(2, 1));
        Aver.AreObjectsEqual(b, img.GetPixel(5, 1));
        Aver.AreObjectsEqual(g, img.GetPixel(7, 1));
        Aver.AreObjectsEqual(b, img.GetPixel(1, 2));
        Aver.AreObjectsEqual(b, img.GetPixel(3, 2));
        Aver.AreObjectsEqual(r, img.GetPixel(5, 2));
        Aver.AreObjectsEqual(b, img.GetPixel(2, 3));
        Aver.AreObjectsEqual(r, img.GetPixel(4, 3));
        Aver.AreObjectsEqual(g, img.GetPixel(6, 3));
      }

    #endregion
  }
}
