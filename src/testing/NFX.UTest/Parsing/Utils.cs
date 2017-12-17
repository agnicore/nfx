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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NFX.Scripting;

using NFX.Parsing;


namespace NFX.UTest.Parsing
{
    [Runnable]   
    public class Utils
    {
        [Run]
        public void FieldNameToDescription()
        {
          Aver.AreEqual("First Name", "FIRST_NAME".ParseFieldNameToDescription(true));
          Aver.AreEqual("first name", "FIRST_NAME".ParseFieldNameToDescription(false));

          Aver.AreEqual("First Name 2", "FIRST-NAME_2".ParseFieldNameToDescription(true));
          Aver.AreEqual("first name 2", "FIRST-NAME_2".ParseFieldNameToDescription(false));

          Aver.AreEqual("First Name 013 S", "FirstName013S".ParseFieldNameToDescription(true));
          Aver.AreEqual("first name 013 s", "FirstName013S".ParseFieldNameToDescription(false));

          Aver.AreEqual("turn off", "TurnOFF".ParseFieldNameToDescription(false));
          Aver.AreEqual("Turn Off", "TurnOFF".ParseFieldNameToDescription(true));

          Aver.AreEqual("first name", "__FIRST__Name".ParseFieldNameToDescription(false));
          Aver.AreEqual("First Name", "__FIRST__Name__".ParseFieldNameToDescription(true));

          Aver.AreEqual("field 0 1", "field_0_1".ParseFieldNameToDescription(false));
          Aver.AreEqual("Field 0 1", "field_0_1".ParseFieldNameToDescription(true));
        }


        [Run]
        public void MatchPattern1()
        {
          Aver.IsTrue( "some address".MatchPattern("s?me?addres?") );
          Aver.IsTrue( "same-addresZ".MatchPattern("s?me?addres?") );
        }

        [Run]
        public void MatchPattern2()
        {
          Aver.IsTrue ( "some address".MatchPattern("s?me?addres?", senseCase: true) );
          Aver.IsFalse( "same-addreZs".MatchPattern("s?me?addres?", senseCase: true) );
        }


        [Run]
        public void MatchPattern3()
        {
          Aver.IsTrue( "some address".MatchPattern("some*") );
        }
        [Run]
        public void MatchPattern4()
        {
          Aver.IsTrue( "some address".MatchPattern("s?me*") );
        }

        [Run]
        public void MatchPattern5()
        {
          Aver.IsTrue( "some address".MatchPattern("s?me*addre??") );
        }

        [Run]
        public void MatchPattern6()
        { 
          Aver.IsTrue( "same Address".MatchPattern("s?me*addre??") );
        } 

        [Run]
        public void MatchPattern7()
        { 
          Aver.IsTrue( "same AddreZZ".MatchPattern("s?me*addre??") );
          Aver.IsFalse( "same AddreZZ?".MatchPattern("s?me*addre??") );
          Aver.IsFalse( "same AddreZ"  .MatchPattern("s?me*addre??") );
        }

        [Run]
        public void MatchPattern8()
        { 
          Aver.IsTrue( "same AddreZZ".MatchPattern("*") );
        }

        [Run]
        public void MatchPattern9()
        { 
          Aver.IsFalse( "same AddreZZ".MatchPattern("") );
        }

        [Run]
        public void MatchPattern10()
        { 
          Aver.IsFalse( "same AddreZZ".MatchPattern("?") );
        }

        [Run]
        public void MatchPattern11()
        { 
          Aver.IsTrue( "same AddreZZ".MatchPattern("????????????") );
        }

        [Run]
        public void MatchPattern12()
        { 
          Aver.IsTrue( "same AddreZZ".MatchPattern("same*") );
        }

        [Run]
        public void MatchPattern13()
        { 
          Aver.IsTrue( "same AddreZZ".MatchPattern("*addre??") );
        }

        [Run]
        public void MatchPattern14()
        { 
          Aver.IsTrue( "same Address".MatchPattern("*address") );
        }

        [Run]
        public void MatchPattern15_1()
        { 
          Aver.IsTrue( "some same crazy address address Address".MatchPattern("*address") );
        }

        [Run]
        public void MatchPattern15_2()
        { 
          Aver.IsFalse( "some same crazy address address Address".MatchPattern("*address", senseCase: true) );
        }

        [Aver.Throws(typeof(NFXException), Message="contains more than one", MsgMatch=Aver.ThrowsAttribute.MatchType.Contains)]
        [Run]
        public void MatchPattern16_1()
        { 
          "some crazy address".MatchPattern("*crazy*");
        }

        [Aver.Throws(typeof(NFXException), Message="contains more than one", MsgMatch=Aver.ThrowsAttribute.MatchType.Contains)]
        [Run]
        public void MatchPattern16_2()
        { 
          "some crazy address".MatchPattern("cr*azy*");
        }


        [Aver.Throws(typeof(NFXException), Message="contains more than one", MsgMatch=Aver.ThrowsAttribute.MatchType.Contains)]
        [Run]
        public void MatchPattern16_3()
        { 
          "some crazy address".MatchPattern("*cra*zy");
        }


        [Run]
        public void MatchPattern17()
        { 
          Aver.IsTrue( "127.0.0.1".MatchPattern("127.0.*") );
        }

        [Run]
        public void MatchPattern18()
        { 
          Aver.IsTrue( "https://some-site.com/?q=aaaa".MatchPattern("https://some-site.com*") );
        }

        [Run]
        public void MatchPattern19()
        { 
          Aver.IsTrue( "140.70.81.139".MatchPattern("140.70.81.139") );
        }

        [Run]
        public void MatchPattern20()
        { 
          Aver.IsTrue( "140.70.81.139" .MatchPattern("140.70.*.139") );
          Aver.IsTrue( "140.70.1.139"  .MatchPattern("140.70.*.139") );
          Aver.IsTrue( "140.70.17.139" .MatchPattern("140.70.*.139") );
          Aver.IsTrue( "140.70.123.139".MatchPattern("140.70.*.139") );

          Aver.IsFalse( "141.70.81.139" .MatchPattern("140.70.*.139") );
          Aver.IsFalse( "140.71.1.139"  .MatchPattern("140.70.*.139") );
          Aver.IsFalse( "140.70.17.13"  .MatchPattern("140.70.*.139") );
          Aver.IsFalse( "140.70.123.137".MatchPattern("140.70.*.139") );
        }

        [Run]
        public void CheckScreenName()
        { 
          Aver.IsFalse( DataEntryUtils.CheckScreenName("10o") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("1.0o") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName(".aa") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("2d-2222") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("DIMA-aaaaa..") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("дима 123") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName(".дима 123") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("1дима-123") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("-дима") );
          Aver.IsFalse( DataEntryUtils.CheckScreenName("дима.") );


          Aver.IsTrue(  DataEntryUtils.CheckScreenName("dima-qwerty") );
          Aver.IsTrue(  DataEntryUtils.CheckScreenName("d2-2222") );
          Aver.IsTrue( DataEntryUtils.CheckScreenName("дима123") );
          Aver.IsTrue( DataEntryUtils.CheckScreenName("дима-123") );
          Aver.IsTrue( DataEntryUtils.CheckScreenName("дима.123") );
        }

        [Run]
        public void NormalizePhone1()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("5552224415");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415", n);
        }

        [Run]
        public void NormalizePhone2()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("2224415");
          Console.WriteLine(n);
          Aver.AreEqual("(???) 222-4415", n);
        }

        [Run]
        public void NormalizePhone3()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("   +38 067 2148899   ");
          Console.WriteLine(n);
          Aver.AreEqual("+38 067 2148899", n);
        }

        [Run]
        public void NormalizePhone4()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415", n);
        }

        [Run]
        public void NormalizePhone5()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415 EXT 2014");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x2014", n);
        }

        [Run]
        public void NormalizePhone6()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415.2014");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x2014", n);
        }

        [Run]
        public void NormalizePhone7()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415EXT.2014");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x2014", n);
        }

        [Run]
        public void NormalizePhone8()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415 X 2014");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x2014", n);
        }

        [Run]
        public void NormalizePhone9()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555.222.4415");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415", n);
        }

        [Run]
        public void NormalizePhone10()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("555-222-4415");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415", n);
        }

        [Run]
        public void NormalizePhone11()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("5552224415ext123");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x123", n);
        }

        [Run]
        public void NormalizePhone12()
        { 
          var n = DataEntryUtils.NormalizeUSPhone("5552224415ext.123");
          Console.WriteLine(n);
          Aver.AreEqual("(555) 222-4415x123", n);
        }

        [Run]
        public void CheckEmail()
        {
          string[] validEMails = {
            "user@example.com",
            "user.777@example.com",
            "u.ser@example.com",
            "user@com",
            "user@e.xample",
            "юзер@мояпочта.ры",
            "#!$%&'*+-/=?^_`{}|~@example.com",
            "-user-@example.com",
            "us-_-er@example.com",
            "user#01@example.com",
            "user@7super.puper08.example.com",
            "user@example--com",
            "user@example.s43",
            "user@example.museum",
            "alex.jack.soybean@example.of.my.domain.com.me",
            "boris_zhaba@yahoo.com",
            "boris-zhaba@yahoo.com",
            "boris.zhaba@yahoo.com",
            "boris___zhaba@yahoo.com",
            "sunny2346273864263@yahoo.com"
          };

          Console.WriteLine("==== Valid emails ====");
          foreach (var email in validEMails)
          {
            Console.WriteLine(email);
            Aver.IsTrue(DataEntryUtils.CheckEMail(email));
          }

          Console.WriteLine("==== Invlaid emails ====");
          string[] invalidEMails = {
            "  ",
            "@",
            "user@ ",
            "user@",
            " @example.com",
            "@example.com",
            ".@.",
            "dima@zaza@yahoo.com",
            "dima zaza@yahoo.com",
            "user",
            "user2example.com",
            "user.@example.com",
            ".user@example.com",
            "user@example.com.",
            "user@.example.com",
            "us..er@example.com",
            "user@example..com",
            "user @example.com",
            "user@example.com ",
            "user@ example.com",
            "us er@example.com",
            "user@example com",
            "user@example .com",
            "user@example.-com",
            "user@example-.com",
            "user@-example.com",
            "user@examplecom-",
            "user@e-.xample.com",
            "user@e.-xample.com",
            "us@er@example.com",
            "user#example.com",
            "user@example/com",
            @"us\er@example.com",
            @"user@exa\mple.com",
            "us(er@example.com",
            "user(comment)@example.com",
            "user@exa(mple.com",
            "us)er@example.com",
            "user@exa)mple.com",
            "us,er@example.com",
            "user@exa,mple.com",
            "us:er@example.com",
            "user@exa:mple.com",
            "us;er@example.com",
            "user@exa;mple.com",
            "us<er@example.com",
            "user@exa<mple.com",
            "us>er@example.com",
            "user@exa>mple.com",
            "us[er@example.com",
            "user@exa[mple.com",
            "us]er@example.com",
            "user@exa]mple.com",
            "user@exam-_ple.com"
          };

          foreach (var email in invalidEMails)
          {
            Console.WriteLine(email);
            Aver.IsFalse(DataEntryUtils.CheckEMail(email));
          }
          
        }

        [Run]
        public void CheckPhone()
        {
          string[] good = {
            "(800) 234-2345x234",
            "(800) 234-2345",
            "800 2345678",
            "800 234-4522",
            "800.2345678",
            "800.234.4522",
            "800-234-2345",
            "800-234-2345x234",
            "8882344511",
            "(888)2344511",
            "(888)234-4511",
            "(888)234.4511",
            "(888) 2344511",
            "(888) 234 4511",
            "(900) 4megood",
            "9004megood",
            "+28937498723987498237",
            "+8293 823098 82394",
            "+3423-3423-234-34" ,
            "+3423-3423-234x456",
            "+1 900 4ME-GOOD"
          };
          string[] bad =
          {
            "800",
            "(800)",
            "(8888)234-4511",
            " (888)234-4511",
            "(888)234-4511 ",
            "(8-88)234-4511",
            "+1423423 +23423",
            ")800 23456777(",
            "800)1234567",
            "(216) 234(2345)",
            "345#aaaaa",
            "7567:242333",
            "+800242--3333",
            "+800242..3333",
            "+800242-.3333",
            "#800242.-3333",
            "+800242.-3333",
            "+(80 0)242.-3333",
            "(800).2423333",
            "(800)-2423333",
            "(800)2423333.",
            ".(800)2423333",
            "-(800)2423333",
            "((800))2423333",
            "(800-)2423333",
            "(.800)2423333",
            "+(800)242-3333",
            "(800)242. 3333",
            "(800)242 - 3333",
            "(800)242        COOL",
            "(800)242 - 33 - 33"
          };
          Console.WriteLine("Good numbers:");
          for (int i = 0; i < good.Length; i++)
          {
            Console.WriteLine(good[i]);
            Aver.IsTrue(DataEntryUtils.CheckTelephone(good[i]));
          }

          Console.WriteLine("Bad numbers:");
          for (int i = 0; i < bad.Length; i++)
          {
              Console.WriteLine(bad[i]);
              Aver.IsFalse(DataEntryUtils.CheckTelephone(bad[i]));
          }
        }
    }
  }


