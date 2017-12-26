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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NFX.Scripting;

using NFX.Environment;
using NFX.ApplicationModel;

namespace NFX.UTest.AppModel
{
    [Runnable(TRUN.BASE, 3)]
    public class FinishNotifiablesTests
    {

        public static string RESULT;


        [Run]
        public void StartFinish()
        {
            var confSource=@" nfx{  starters{ starter{ type='NFX.UTest.AppModel.MySuperStarter, NFX.UTest'} }    }";
            RESULT = "";
            var conf = LaconicConfiguration.CreateFromString(confSource);
            using( var app = new ServiceBaseApplication(null, conf.Root))
            {

            }

            Aver.AreEqual("ABCD", RESULT);

        }



    }


            public class MySuperStarter: IApplicationStarter
            {

                public bool ApplicationStartBreakOnException
                {
                    get { return false; }
                }

                public void ApplicationStartBeforeInit(IApplication application)
                {
                   FinishNotifiablesTests.RESULT += "A";
                }

                public void ApplicationStartAfterInit(IApplication application)
                {
                   FinishNotifiablesTests.RESULT += "B";
                   application.RegisterAppFinishNotifiable( new SuperEnder());
                }

                public void Configure(IConfigSectionNode node)
                {

                }

                public string Name
                {
                    get { return "SuperStarter"; }
                }
            }

            public class SuperEnder : IApplicationFinishNotifiable
            {

                public void ApplicationFinishBeforeCleanup(IApplication application)
                {
                    FinishNotifiablesTests.RESULT += "C";
                }

                public void ApplicationFinishAfterCleanup(IApplication application)
                {
                    FinishNotifiablesTests.RESULT += "D";
                }

                public string Name
                {
                    get { return "SuperEnder"; }
                }
            }


}