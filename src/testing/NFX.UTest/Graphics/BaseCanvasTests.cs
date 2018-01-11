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
  public class BaseCanvasTests : GraphicsTestBase
  {
    [Run]
    public void Canvas_DrawImageUnscaled()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        canvas.DrawImageUnscaled(TestImg2, 2, 0);

        for (int h=0; h<3; h++)
        for (int w=0; w<3; w++)
          Aver.AreObjectsEqual(TestImg2.GetPixel(w, h), TestImg1.GetPixel(w+2,h));

        canvas.DrawImageUnscaled(TestImg2, new Point(6, 2));

        for (int h=0; h<2; h++)
        for (int w=0; w<2; w++)
          Aver.AreObjectsEqual(TestImg2.GetPixel(w, h), TestImg1.GetPixel(w+6,h+2));

        canvas.DrawImageUnscaled(TestImg2, 0.6F, 2.1F);

        for (int h=0; h<2; h++)
        for (int w=0; w<3; w++)
          Aver.AreObjectsEqual(TestImg2.GetPixel(w, h), TestImg1.GetPixel(w,h+2));

        canvas.DrawImageUnscaled(TestImg2, new PointF(4.7F, -1.7F));

        for (int h=1; h<3; h++)
        for (int w=0; w<3; w++)
          Aver.AreObjectsEqual(TestImg2.GetPixel(w, h), TestImg1.GetPixel(w+4,h-1));
      }
    }

    [Run]
    public void Canvas_DrawImage()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        canvas.DrawImage(TestImg2, 0, 0, 6, 6);

        Aver.AreObjectsEqual(G, TestImg1.GetPixel(0, 0));
        Aver.AreObjectsEqual(B, TestImg1.GetPixel(2, 0));
        Aver.AreObjectsEqual(R, TestImg1.GetPixel(4, 0));
        Aver.AreObjectsEqual(B, TestImg1.GetPixel(6, 0));
        Aver.AreObjectsEqual(G, TestImg1.GetPixel(7, 0));
        Aver.AreObjectsEqual(G, TestImg1.GetPixel(0, 2));
        Aver.AreObjectsEqual(B, TestImg1.GetPixel(2, 2));
      }
    }

    [Run]
    public void Canvas_Clear()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        var clr = Color.FromArgb(12, 34, 120);
        canvas.Clear(clr);

        for (int h=0; h<TestImg2.Height; h++)
        for (int w=0; w<TestImg2.Width; w++)
          Aver.AreObjectsEqual(clr, TestImg1.GetPixel(w, h));
      }
    }

    [Run]
    public void Canvas_FillRectangle()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        var clr = Color.FromArgb(12, 34, 120);
        var br = canvas.CreateSolidBrush(clr);
        canvas.FillRectangle(br, 1, 1, 2, 3);

        for (int h=0; h<TestImg2.Height; h++)
        for (int w=0; w<TestImg2.Width; w++)
        {
          if (w>=1 && w<=2 && h>=1 && h<=3)
            Aver.AreObjectsEqual(clr, TestImg1.GetPixel(w, h));
          else
            Aver.AreObjectsNotEqual(clr, TestImg1.GetPixel(w, h));
        }
      }
    }

    [Run]
    public void Canvas_DrawRectangle()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        var clr = Color.FromArgb(12, 34, 120);
        var pen = canvas.CreatePen(clr, 1, PenDashStyle.Solid);
        canvas.DrawRectangle(pen, 1, 1, 5, 3);

        for (int h=0; h<TestImg2.Height; h++)
        for (int w=0; w<TestImg2.Width; w++)
        {
          if (w>=1 && w<=5 && h>=1 && h<=3 && (w==1 || w==5 || h==1 || h==3))
            Aver.AreObjectsEqual(clr, TestImg1.GetPixel(w, h));
          else
            Aver.AreObjectsNotEqual(clr, TestImg1.GetPixel(w, h));
        }
      }
    }

    [Run]
    public void Canvas_DrawLine()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        var clr = Color.FromArgb(12, 34, 120);
        var pen = canvas.CreatePen(clr, 1, PenDashStyle.Solid);
        canvas.DrawLine(pen, 1, 1, 3, 3);

        for (int h=0; h<TestImg2.Height; h++)
        for (int w=0; w<TestImg2.Width; w++)
        {
          if (w>=1 && w<=3 && w==h)
            Aver.AreObjectsEqual(clr, TestImg1.GetPixel(w, h));
          else
            Aver.AreObjectsNotEqual(clr, TestImg1.GetPixel(w, h));
        }
      }
    }

    [Run]
    public void Canvas_DrawEllipse()
    {
      using (var canvas = TestImg1.CreateCanvas())
      {
        var clr = Color.FromArgb(12, 34, 120);
        var pen = canvas.CreatePen(clr, 1, PenDashStyle.Solid);
        canvas.DrawEllipse(pen, 1, 1, 3, 2);

        for (int h=0; h<TestImg2.Height; h++)
        for (int w=0; w<TestImg2.Width; w++)
        {
          if ((h==1 && (w==2 || w==3)) || (h==2 && (w==1 || w==4)) || (h==3 && (w==2 || w==3)))
            Aver.AreObjectsEqual(clr, TestImg1.GetPixel(w, h));
          else
            Aver.AreObjectsNotEqual(clr, TestImg1.GetPixel(w, h));
        }
      }
    }

  }
}
