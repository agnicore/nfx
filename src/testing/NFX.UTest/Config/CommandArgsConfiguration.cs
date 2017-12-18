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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NFX.Scripting;


using CAC = NFX.Environment.CommandArgsConfiguration;

namespace NFX.UTest.Config
{
    [Runnable]
    public class CmdArgsConfiguration
    {
        private string[] args = {
           @"tool.exe",
           @"c:\input.file",
           @"d:\output.file",
           @"-compress",
           @"level=100",
           @"method=zip",
           @"-shadow",
           @"fast",
           @"swap=1024",
           @"-large"
         };


        [Run]
        public void GeneralCmdArgs()
        {
          var conf = new CAC(args);

          Aver.AreEqual(@"tool.exe", conf.Root.AttrByIndex(0).ValueAsString());
          Aver.AreEqual(@"c:\input.file", conf.Root.AttrByIndex(1).ValueAsString());
          Aver.AreEqual(@"d:\output.file", conf.Root.AttrByIndex(2).ValueAsString());

          Aver.AreEqual(true, conf.Root["compress"].Exists);
          Aver.AreEqual(100, conf.Root["compress"].AttrByName("level").ValueAsInt());
          Aver.AreEqual("zip", conf.Root["compress"].AttrByName("method").ValueAsString());

          Aver.AreEqual(true, conf.Root["shadow"].Exists);
          Aver.AreEqual("fast", conf.Root["shadow"].AttrByIndex(0).Value);
          Aver.AreEqual(1024, conf.Root["shadow"].AttrByName("swap").ValueAsInt());

          Aver.AreEqual(true, conf.Root["large"].Exists);
        }

    }
}

