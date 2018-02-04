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
  public class ColorXlatTests
  {
    [Run]
    public void ToHTML()
    {
      var color = Color.Empty;
      var html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#000", html);

      color = Color.White;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#FFFFFF", html);

      color = Color.Black;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#000000", html);

      color = Color.Red;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#FF0000", html);

      color = Color.Green;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#008000", html);

      color = Color.Lime;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#00FF00", html);

      color = Color.Blue;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#0000FF", html);

      color = Color.Yellow;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#FFFF00", html);

      color = Color.Magenta;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#FF00FF", html);

      color = Color.Aqua;
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#00FFFF", html);

      color = Color.FromArgb(128, 128, 128);
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#808080", html);

      color = Color.FromArgb(42, 10, 8);
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#2A0A08", html);

      color = Color.FromArgb(153, 153, 153);
      html = ColorXlat.ToHTML(color);
      Aver.AreEqual("#999999", html);
    }

    [Run]
    public void FromHTML()
    {
      var html = string.Empty;
      var color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Empty, color);

      html = "#FFFFFF";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      html = "#FFF";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      html = "#ffF";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      html = "#fFFffF";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      html = "white";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      html = "#f00";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Red, color);

      html = "#00Ff00";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Lime, color);

      html = "green";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Green, color);

      html = "#0000ff";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Blue, color);

      html = "#Ff0";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Yellow, color);

      html = "#888";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.FromArgb(136, 136, 136), color);

      html = "#Ccc";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.FromArgb(204, 204, 204), color);

      html = "#80Abe9";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.FromArgb(128, 171, 233), color);
    }

    [Run]
    public void FromHTMLNegative()
    {
      var html = "FFFFFF";
      var color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.Empty, color);

      html = "#fffz";
      color = ColorXlat.FromHTML(html);
      colorAreEqual(Color.White, color);

      try
      {
        html = "##fff";
        color = ColorXlat.FromHTML(html);
        Aver.Fail("ArgumentException is expected");
      }
      catch (Exception ex)
      {
        Aver.IsTrue(ex is ArgumentException);
      }
    }

    private void colorAreEqual(Color c1, Color c2)
    {
      Aver.AreEqual(c1.R, c2.R);
      Aver.AreEqual(c1.G, c2.G);
      Aver.AreEqual(c1.B, c2.B);
    }
  }
}
