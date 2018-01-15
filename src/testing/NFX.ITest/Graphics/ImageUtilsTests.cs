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
  public class ImageUtilsTests : GraphicsTestBase
  {
    #region CONSTS

      public const string TEST_JPG = "test_mc.jpg";
      public const string TEST_PNG = "test_mc.png";

    #endregion

    [Run]
    public void ExtractMainColors_TestPNG()
    {
      using (var stream = GetResource(TEST_PNG))
      using (var img = Image.FromStream(stream))
      {
        var colors = ImageUtils.ExtractMainColors(img);

        Aver.AreEqual(4, colors.Length);
        Aver.AreObjectsEqual(Color.FromArgb(240, 190, 2),   colors[0]);
        Aver.AreObjectsEqual(Color.FromArgb(147, 213, 206), colors[1]);
        Aver.AreObjectsEqual(Color.FromArgb(26, 168, 74),   colors[2]);
        Aver.AreObjectsEqual(Color.FromArgb(144, 72, 143),  colors[3]);
      }
    }

    [Run]
    public void ExtractMainColors_TestJPG()
    {
      using (var stream = GetResource(TEST_JPG))
      using (var img = Image.FromStream(stream))
      {
        var colors = ImageUtils.ExtractMainColors(img);

        Aver.AreEqual(4, colors.Length);
        Aver.AreObjectsEqual(Color.FromArgb(216, 24, 25),   colors[0]);
        Aver.AreObjectsEqual(Color.FromArgb(55, 64, 176),   colors[1]);
        Aver.AreObjectsEqual(Color.FromArgb(185, 210, 30),  colors[2]);
        Aver.AreObjectsEqual(Color.FromArgb(240, 240, 232), colors[3]);
      }
    }
  }
}
