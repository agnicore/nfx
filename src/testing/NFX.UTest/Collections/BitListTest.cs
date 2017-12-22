/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2017 ITAdapter Corp. Inc.
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
using NFX.Collections;
using NFX.Scripting;

namespace NFX.UTest.Collections
{
  [Runnable]
  public class BitListTest
  {
    [Run]
    [Aver.Throws(typeof(ArgumentOutOfRangeException))]
    public void OutOfRange()
    {
      BitList bits = new BitList();
      var b = bits[0];
    }

    [Run]
    public void AppendBit()
    {
      BitList bits = new BitList();

      Aver.AreEqual(0, bits.Size);

      bits.AppendBit(true);

      Aver.AreEqual(1, bits.Size);
      Aver.AreEqual(true, bits[0]);

      bits.AppendBit(false);
      bits.AppendBit(true);
      Aver.AreEqual(3, bits.Size);
      Aver.AreEqual(false, bits[1]);
      Aver.AreEqual(true, bits[2]);
    }

    [Run]
    public void AppendOther()
    {
      const int SIZE0 = 13, SIZE1 = 33;

      BitList bits0 = new BitList();
      for (int i = 0; i < SIZE0; i++)
        bits0.AppendBit(true);

      BitList bits1 = new BitList();
      for (int i = 0; i < SIZE1; i++)
        bits1.AppendBit(true);

      bits0.AppendBitList(bits1);

      Aver.AreEqual( SIZE0 + SIZE1, bits0.Size);
    }

  }
}
