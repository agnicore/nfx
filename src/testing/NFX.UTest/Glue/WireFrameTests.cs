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

using NFX.Scripting;
using NFX.Glue.Native;
using System.IO;
using NFX.Glue;
using NFX.IO;

namespace NFX.UTest.Glue
{
    [Runnable(TRUN.BASE)]
    public class WireFrameTests
    {
        [Run]
        public void Glue_SerializeDeserialize()
        {
            var frm1 = new WireFrame(123, false, FID.Generate());

            Aver.AreEqual( WireFrame.FRAME_LENGTH, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsFalse( frm2.OneWay );

        }

        [Run]
        public void Glue_SerializeDeserialize_WithHeadersWithLatinText()
        {
            var hdr = "<a><remote name='zzz'/></a>";//Latin only chars

            var frm1 = new WireFrame(123, false, FID.Generate(), hdr);


            var utfLen = WireFrame.HEADERS_ENCODING.GetByteCount( hdr );

            Aver.IsTrue( utfLen == hdr.Length);

            Aver.AreEqual( WireFrame.FRAME_LENGTH + hdr.Length, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH + hdr.Length, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsFalse( frm2.OneWay );

            Aver.AreEqual( "zzz", frm2.Headers["remote"].AttrByName("name").Value);
        }

        [Run]
        public void Glue_SerializeDeserialize_WithHeadersWithChineseText()
        {
            var hdr = "<a><remote name='久有归天愿'/></a>";

            var frm1 = new WireFrame(123, false, FID.Generate(), hdr);


            var utfLen = WireFrame.HEADERS_ENCODING.GetByteCount( hdr );

            Aver.IsTrue( utfLen > hdr.Length);
            Console.WriteLine("{0} has {1} byte len and {2} char len".Args(hdr, utfLen, hdr.Length) );

            Aver.AreEqual( WireFrame.FRAME_LENGTH + utfLen, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH + utfLen, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsFalse( frm2.OneWay );

            Aver.AreEqual( "久有归天愿", frm2.Headers["remote"].AttrByName("name").Value);
        }



        [Run]
        public void Echo_SerializeDeserialize()
        {
            var frm1 = new WireFrame(FrameType.Echo, 123, true, FID.Generate());
            Aver.IsFalse( frm1.OneWay );
            Aver.AreEqual( WireFrame.FRAME_LENGTH, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsFalse( frm2.OneWay );
        }

        [Run]
        public void Dummy_SerializeDeserialize()
        {
            var frm1 = new WireFrame(FrameType.Dummy, 123, false, FID.Generate());
            Aver.IsTrue( frm1.OneWay );
            Aver.AreEqual( WireFrame.FRAME_LENGTH, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsTrue( frm2.OneWay );
        }

        [Run]
        public void EchoResponse_SerializeDeserialize()
        {
            var frm1 = new WireFrame(FrameType.EchoResponse, 123, false, FID.Generate());
            Aver.IsTrue( frm1.OneWay );
            Aver.AreEqual( WireFrame.FRAME_LENGTH, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsTrue( frm2.OneWay );
        }

        [Run]
        public void HeartBeat_SerializeDeserialize()
        {
            var frm1 = new WireFrame(FrameType.Heartbeat, 123, false, FID.Generate());
            Aver.IsTrue( frm1.OneWay );
            Aver.AreEqual( WireFrame.FRAME_LENGTH, frm1.Length );

            var ms = new MemoryStream();

            Aver.AreEqual(WireFrame.FRAME_LENGTH, frm1.Serialize(ms));

            ms.Position = 0;

            var frm2 = new WireFrame(ms);

            Aver.IsTrue( frm1.Type == frm2.Type );
            Aver.AreEqual( frm1.RequestID, frm2.RequestID );
            Aver.AreEqual( frm1.OneWay, frm2.OneWay );

            Aver.AreEqual( frm1.Length, frm2.Length );
            Aver.AreEqual( frm1.Format, frm2.Format );
            Aver.AreEqual( frm1.HeadersContent, frm2.HeadersContent );

            Aver.IsTrue( frm2.OneWay );
        }


    }
}
