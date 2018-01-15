
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

/* NFX by ITAdapter
 * Originated: 2006.01
 * Revision: NFX 5.0  2018.01.15
 * Based on zXing / Apache 2.0; See NOTICE and CHANGES for attribution
 */

using System.Drawing;


using NFX.Graphics;

namespace NFX.Media.TagCodes.QR
{

  public static class QRImageRenderer
  {
    #region CONSTS

      private static Color DEFAULT_TRUE_COLOR = Color.FromArgb(0, 0, 0);
      private static Color DEFAULT_FALSE_COLOR = Color.FromArgb(0, 0, 0);

    #endregion

    #region Inner Types

      public enum ImageScale { Scale1x = 1, Scale2x = 2, Scale3x = 3, Scale4x = 4, Scale5x = 5, Scale6x = 6, Scale7x = 7, Scale8x = 8};

    #endregion

    #region Public

      public static void ToBMP(this QRMatrix matrix,
                               System.IO.Stream stream,
                               Color? trueColor = null,
                               Color? falseColor = null,
                               ImageScale? scale = ImageScale.Scale1x)
      {
        var output = createOutput(matrix, trueColor, falseColor, scale);
        output.Save(stream, BitmapImageFormat.Monochrome);
      }

      public static void ToPNG(this QRMatrix matrix,
                               System.IO.Stream stream,
                               Color? trueColor = null,
                               Color? falseColor = null,
                               ImageScale? scale = ImageScale.Scale1x)
      {
        var output = createOutput(matrix, trueColor, falseColor, scale);
        output.Save(stream, PngImageFormat.Monochrome);
      }

      public static void ToJPG(this QRMatrix matrix,
                               System.IO.Stream stream,
                               Color? trueColor = null,
                               Color? falseColor = null,
                               ImageScale? scale = ImageScale.Scale1x)
      {
        var output = createOutput(matrix, trueColor, falseColor, scale);
        output.Save(stream, JpegImageFormat.Standard);
      }

      public static void ToGIF(this QRMatrix matrix,
                               System.IO.Stream stream,
                               Color? trueColor = null,
                               Color? falseColor = null,
                               ImageScale? scale = ImageScale.Scale1x)
      {
        var output = createOutput(matrix, trueColor, falseColor, scale);
        output.Save(stream, GifImageFormat.Monochrome);
      }

      public static Image CreateOutput(this QRMatrix matrix,
                                       Color? trueColor = null,
                                       Color? falseColor = null,
                                       ImageScale? scale = ImageScale.Scale1x)
      {
        return createOutput(matrix, trueColor, falseColor, scale);
      }

    #endregion

    #region .pvt. impl.

      private static Image createOutput(QRMatrix matrix,
                                        Color? trueColor = null,
                                        Color? falseColor = null,
                                        ImageScale? scale = ImageScale.Scale1x)
      {
        var black = trueColor ?? Color.Black;
        var white = falseColor ?? Color.White;

        if (black == white)
          throw new NFXException(StringConsts.ARGUMENT_ERROR + typeof(QRImageRenderer).Name + ".ToBitmap(trueColor!=falseColor)");

        int scaleFactor = (int)scale;

        int canvasWidth = matrix.Width * scaleFactor;
        int canvasHeight = matrix.Height * scaleFactor;

        var result = Image.Of(canvasWidth, canvasHeight);

        using (var canvas = result.CreateCanvas())
        using (var blackBrush = canvas.CreateSolidBrush(black))
        using (var whiteBrush = canvas.CreateSolidBrush(white))
        {
          canvas.FillRectangle(blackBrush, 0, 0, canvasWidth, canvasHeight);

          for (int yMatrix=0; yMatrix<matrix.Height; yMatrix++)
          for (int xMatrix=0; xMatrix<matrix.Width;  xMatrix++)
          {
            if (matrix[xMatrix, yMatrix] == 0)
            {
               int scaledX = xMatrix * scaleFactor;
               int scaledY = yMatrix * scaleFactor;

               canvas.FillRectangle(whiteBrush, scaledX, scaledY, scaleFactor, scaleFactor);
            }
          }

          return result;
        }
      }

    #endregion

  }//class

}
