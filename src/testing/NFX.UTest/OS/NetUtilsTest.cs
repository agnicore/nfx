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
using System.IO;
using System.Linq;
using System.Text;

using NFX.Scripting;

using NFX.OS;
using NFX.Serialization.JSON;

namespace NFX.UTest.OS
{

    [Runnable]
    public class NetUtilsTest
    {
        [Run]
        public void GetUniqueNetworkSignature()
        {
            var sig = NetworkUtils.GetMachineUniqueMACSignature();

            Console.WriteLine( sig );

            if (System.Environment.MachineName=="SEXTOD")
             Aver.AreEqual("9-08CC2E06-DD8F620D34473E5E", sig);
            else
             Aver.Fail("This test can only run on SEXTOD");
        }

        [Run]
        public void Computer_UniqueNetworkSignature()
        {
            var sig = Computer.UniqueNetworkSignature;

            Console.WriteLine( sig );

            if (System.Environment.MachineName=="SEXTOD")
             Aver.AreEqual("9-08CC2E06-DD8F620D34473E5E", sig);
            else
             Aver.Fail("This test can only run on SEXTOD");
        }

        [Run]
        public void HostNetInfo_1()
        {
            var hi = HostNetInfo.ForThisHost();

            Console.WriteLine(  hi.ToJSON(JSONWritingOptions.PrettyPrint) );

            if (System.Environment.MachineName=="SEXTOD")
             Aver.IsTrue( hi.Adapters["{C93B4009-15C0-46A3-8C95-91610CAEBC4F}::Local Area Connection"].Addresses.ContainsName("192.168.1.70") );
            else
             Aver.Fail("This test can only run on SEXTOD");
        }
    }
}
