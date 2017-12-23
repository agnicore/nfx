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

using NFX.Scripting;


using NFX.ApplicationModel;
using NFX.Log;

namespace NFX.UTest.Config
{
    [Runnable]
    public class ValueAccessors
    {

    static string conf = @"
 test
 {
   vInt1=123
   vInt2=-123
   vDouble=-123.8002341
   vDecimal=123000456.1233
   vHex=0xABAB
   vBin=0b1010101001010101 //AA55
   vBool=true
   vStr=$'My
   name
   spanning many lines'
   vDate=12/10/2014
   vGuid1='{3A7C4641-B24E-453D-9D28-93D96071B575}'
   vGuid2='3A7C4641-B24E-453D-9D28-93D96071B575'
   vGuid3='3A7C4641B24E453D9D2893D96071B575'
   vBuffer=fa,CA,dA,Ba
 }
";

        [Run]
        public void Ints()
        {
            var root = conf.AsLaconicConfig(handling: ConvertErrorHandling.Throw);//throw needed so we can see error (if any) while processing config
                                                                                  // instead of just getting null back

            Aver.AreEqual(123, root.AttrByName("vInt1").ValueAsInt());
            Aver.AreEqual(-123, root.AttrByName("vInt2").ValueAsInt());

        }

        [Run]
        public void Doubles()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(-123.8002341d, root.AttrByName("vDouble").ValueAsDouble());
        }

        [Run]
        public void Decimals()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(123000456.1233M, root.AttrByName("vDecimal").ValueAsDecimal());
        }

        [Run]
        public void HexIntegers()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual((ushort)0xabab, root.AttrByName("vHex").ValueAsUShort());
            Aver.AreEqual((uint)0xabab, root.AttrByName("vHex").ValueAsUInt());
            Aver.AreEqual((ulong)0xabab, root.AttrByName("vHex").ValueAsULong());
        }

        [Run]
        public void BinIntegers()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual((ushort)0xaa55, root.AttrByName("vBin").ValueAsUShort());
            Aver.AreEqual((uint)0xaa55, root.AttrByName("vBin").ValueAsUInt());
            Aver.AreEqual((ulong)0xaa55, root.AttrByName("vBin").ValueAsULong());
        }

        [Run]
        public void Bools()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(true, root.AttrByName("vBool").ValueAsBool());
        }

        [Run]
        public void Strs()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(@"My
   name
   spanning many lines", root.AttrByName("vStr").ValueAsString());
        }

        [Run]
        public void Dates()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(2014, root.AttrByName("vDate").ValueAsDateTime(DateTime.Now).Year);
            Aver.AreEqual(12, root.AttrByName("vDate").ValueAsDateTime(DateTime.Now).Month);
        }

        [Run]
        public void Guids()
        {
            var root = conf.AsLaconicConfig();

            Aver.AreEqual(new Guid("3A7C4641B24E453D9D2893D96071B575"), root.AttrByName("vGUID1").ValueAsGUID(Guid.Empty));
            Aver.AreEqual(new Guid("3A7C4641B24E453D9D2893D96071B575"), root.AttrByName("vGUID2").ValueAsGUID(Guid.Empty));
            Aver.AreEqual(new Guid("3A7C4641B24E453D9D2893D96071B575"), root.AttrByName("vGUID3").ValueAsGUID(Guid.Empty));
        }

        [Run]
        public void ByteArray()
        {
            var root = conf.AsLaconicConfig();

            Aver.IsTrue(new byte[]{0xFA, 0xCA, 0xDA, 0xBA}.SequenceEqual(  root.AttrByName("vBuffer").ValueAsByteArray() ) );
        }

   }//class

}
