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

using NFX.Environment;

namespace NFX.UTest.Config
{
    [Runnable]
    public class FactoryUtilsTests
    {

        [Run]
        public void MakeUsingCtor_1()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>("node{arg0=1 arg1=2}".AsLaconicConfig());
          Aver.AreEqual(1, made.A);
          Aver.AreEqual(2, made.B);
        }

        [Run]
        public void MakeUsingCtor_2()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>("node{arg0='7/1/1982'}".AsLaconicConfig());
          Aver.AreEqual(1982, made.A);
          Aver.AreEqual(7, made.B);
        }

        [Run]
        public void MakeUsingCtor_3()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>("node{type='NFX.UTest.Config.CTORClassDerived, NFX.UTest' arg0=1 arg1=2}".AsLaconicConfig());

          Aver.IsTrue( made is CTORClassDerived);
          Aver.AreEqual(1, made.A);
          Aver.AreEqual(2, made.B);
        }

        [Run]
        public void MakeUsingCtor_4()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>("node{type='NFX.UTest.Config.CTORClassDerived, NFX.UTest' arg0='7/1/1982'}".AsLaconicConfig());

          Aver.IsTrue( made is CTORClassDerived);
          Aver.AreEqual(1982, made.A);
          Aver.AreEqual(7, made.B);
          Aver.AreEqual(155, ((CTORClassDerived)made).C);
        }


        [Run]
        [Aver.Throws(typeof(ConfigException), Message="MakeUsingCtor")]
        public void MakeUsingCtor_5()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>("node{arg0=1}".AsLaconicConfig());
        }


        [Run]
        public void MakeUsingCtor_6()
        {
          var made = FactoryUtils.MakeAndConfigure<CTORClass>(
              "node{type='NFX.UTest.Config.CTORClassDerived, NFX.UTest' data1='AAA' data2='bbb'}".AsLaconicConfig(), args: new object[]{1,12});

          Aver.IsTrue( made is CTORClassDerived);
          Aver.AreEqual(1, made.A);
          Aver.AreEqual(12, made.B);
          Aver.AreEqual("AAA", ((CTORClassDerived)made).Data1);
          Aver.AreEqual("bbb", ((CTORClassDerived)made).Data2);
        }


        [Run]
        public void MakeUsingCtor_7_typePattern()
        {
          var made = FactoryUtils.MakeUsingCtor<CTORClass>(
              "node{type='CTORClassDerived' arg0='12' arg1='234'}".AsLaconicConfig(), typePattern: "NFX.UTest.Config.*, NFX.UTest");

          Aver.IsTrue( made is CTORClassDerived);
          Aver.AreEqual(12, made.A);
          Aver.AreEqual(234, made.B);
        }


    }


       public class CTORClass: IConfigurable
       {
          public CTORClass(int a,int b) { A = a; B = b; }
          public CTORClass(DateTime dt) { A = dt.Year; B = dt.Month; }
          public readonly int A; public readonly int B;

          public void Configure(IConfigSectionNode node)
          {
            ConfigAttribute.Apply(this, node);
          }
       }

       public class CTORClassDerived : CTORClass
       {
          public CTORClassDerived(int a,int b) : base(a,b){}
          public CTORClassDerived(DateTime dt) : base(dt){}

          public readonly int C = 155;

          [Config]
          public string Data1{get;set;}

          [Config]
          public string Data2;
       }

}
