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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NFX.Media.TagCodes.QR;
using NFX.Scripting;

namespace NFX.UTest.Media.TagCodes.QR
{
  [Runnable]
  public class QRImageRendererTest
  {
    [Run]
    public void RenderBMP()
    {
      var matrix = QREncoderMatrix.Encode("ABCDEF", QRCorrectionLevel.H);

      using (var stream = new System.IO.FileStream("ABCDEF.bmp", System.IO.FileMode.Create))
      {
        matrix.ToBMP(stream, scale: QRImageRenderer.ImageScale.Scale4x);
        stream.Flush();
      }
    }

    [Run]
    public void RenderGIF()
    {
      var matrix = QREncoderMatrix.Encode("www.sl.com/BMW-Z3", QRCorrectionLevel.H);

      using (var stream = new System.IO.FileStream("BMW-Z3.gif", System.IO.FileMode.Create))
      {
        matrix.ToGIF(stream, scale: QRImageRenderer.ImageScale.Scale4x);
        stream.Flush();
      }
    }
  }
}
