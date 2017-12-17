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
using System.IO;

using NFX.Scripting;

using NFX.Glue.Native;
using NFX.Glue;
using NFX.IO;

namespace NFX.UTest.Glue
{
    [Runnable]
    public class NodeTests
    {
        [Run]
        public void Node1()
        {
            var n = new Node("http://server:9045");
            Aver.AreEqual("server", n.Host);
            Aver.AreEqual("http", n.Binding);
            Aver.AreEqual("9045", n.Service);
        }

        [Run]
        public void Node2()
        {
            var n = new Node("http://server=127.0.0.1;interface=eth0:hgov");
            Aver.AreEqual("server=127.0.0.1;interface=eth0", n.Host);
            Aver.AreEqual("http", n.Binding);
            Aver.AreEqual("hgov", n.Service);
        }


        [Run]
        public void Node3()
        {
            var n = new Node("server:1891");
            Aver.AreEqual("server", n.Host);
            Aver.AreEqual(string.Empty, n.Binding);
            Aver.AreEqual("1891", n.Service);
        }

        [Run]
        public void Node4()
        {
            var n = new Node("http://server");
            Aver.AreEqual("server", n.Host);
            Aver.AreEqual("http", n.Binding);
            Aver.AreEqual(string.Empty, n.Service);
        }

        [Run]
        public void Node5()
        {
            var n = new Node("server");
            Aver.AreEqual("server", n.Host);
            Aver.AreEqual(string.Empty, n.Binding);
            Aver.AreEqual(string.Empty, n.Service);
        }

    }
}
