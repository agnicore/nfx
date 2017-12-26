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

using NFX.CodeAnalysis;
using NFX.CodeAnalysis.Source;
using NFX.CodeAnalysis.JSON;
using NFX.Serialization.JSON;
using JL=NFX.CodeAnalysis.JSON.JSONLexer;
using JP=NFX.CodeAnalysis.JSON.JSONParser;
using static NFX.Aver.ThrowsAttribute;

namespace NFX.UTest.CodeAnalysis
{
    [Runnable(TRUN.BASE)]
    public class JSONParser
    {

        [Run]
        public void RootLiteral_String()
        {
          var src = @"'abc'";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual("abc", parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_Int()
        {
          var src = @"12";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(12, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_NegativeDecimalInt()
        {
          var src = @"-16";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(-16, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_PositiveDecimalInt()
        {
          var src = @"+16";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(16, parser.ResultContext.ResultObject);
        }


        [Run]
        public void RootLiteral_NegativeHexInt()
        {
          var src = @"-0xf";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(-15, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_PositiveHexInt()
        {
          var src = @"+0xf";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(15, parser.ResultContext.ResultObject);
        }


        [Run]
        public void RootLiteral_Double()
        {
          var src = @"12.7";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(12.7, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_NegativeDouble()
        {
          var src = @"-12.7";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(-12.7, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_PositiveDouble()
        {
          var src = @"+12.7";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(12.7, parser.ResultContext.ResultObject);
        }



        [Run]
        public void RootLiteral_ScientificDouble()
        {
          var src = @"12e2";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(12e2d, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_NegativeScientificDouble()
        {
          var src = @"-12e2";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(-12e2d, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_PositiveScientificDouble()
        {
          var src = @"+12e2";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(12e2d, parser.ResultContext.ResultObject);
        }




        [Run]
        public void RootLiteral_True()
        {
          var src = @"true";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(true, parser.ResultContext.ResultObject);
        }


        [Run]
        public void RootLiteral_False()
        {
          var src = @"false";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.AreObjectsEqual(false, parser.ResultContext.ResultObject);
        }

        [Run]
        public void RootLiteral_Null()
        {
          var src = @"null";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();


          Aver.IsNull(parser.ResultContext.ResultObject);
        }


        [Run]
        public void RootArray()
        {
          var src = @"[1,2,3]";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataArray);
          var arr = (JSONDataArray)parser.ResultContext.ResultObject;

          Aver.AreEqual(3, arr.Count);
          Aver.AreObjectsEqual(1, arr[0]);
          Aver.AreObjectsEqual(2, arr[1]);
          Aver.AreObjectsEqual(3, arr[2]);
        }

        [Run]
        public void RootEmptyArray()
        {
          var src = @"[]";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataArray);
          var arr = (JSONDataArray)parser.ResultContext.ResultObject;

          Aver.AreEqual(0, arr.Count);
        }

        [Run]
        public void RootObject()
        {
          var src = @"{a: 1, b: true, c: null}";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataMap);
          var obj = (JSONDataMap)parser.ResultContext.ResultObject;

          Aver.AreEqual(3, obj.Count);
          Aver.AreObjectsEqual(1, obj["a"]);
          Aver.AreObjectsEqual(true, obj["b"]);
          Aver.IsNull(obj["c"]);
        }

        [Run]
        public void RootEmptyObject()
        {
          var src = @"{}";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataMap);
          var obj = (JSONDataMap)parser.ResultContext.ResultObject;

          Aver.AreEqual(0, obj.Count);
        }

        [Run]
        public void RootObjectWithArray()
        {
          var src = @"{age: 12, numbers: [4,5,6,7,8,9], name: ""Vasya""}";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataMap);
          var obj = (JSONDataMap)parser.ResultContext.ResultObject;

          Aver.AreEqual(3, obj.Count);
          Aver.AreObjectsEqual(12, obj["age"]);
          Aver.AreObjectsEqual("Vasya", obj["name"]);
          Aver.AreObjectsEqual(6, ((JSONDataArray)obj["numbers"]).Count);
          Aver.AreObjectsEqual(7, ((JSONDataArray)obj["numbers"])[3]);
        }

        [Run]
        public void RootObjectWithSubObjects()
        {
          var src = @"{age: 120, numbers: {positive: true, bad: 12.7}, name: ""Vasya""}";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataMap);
          var obj = (JSONDataMap)parser.ResultContext.ResultObject;

          Aver.AreEqual(3, obj.Count);
          Aver.AreObjectsEqual(120, obj["age"]);
          Aver.AreObjectsEqual("Vasya", obj["name"]);
          Aver.AreObjectsEqual(true, ((JSONDataMap)obj["numbers"])["positive"]);
          Aver.AreObjectsEqual(12.7, ((JSONDataMap)obj["numbers"])["bad"]);
        }

        [Run]
        public void ParseError1()
        {
          var src = @"{age 120}";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.AreEqual(1, parser.Messages.Count);
          Aver.AreEqual((int)JSONMsgCode.eColonOperatorExpected,  parser.Messages[0].Code);
        }


        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eColonOperatorExpected", MsgMatch=MatchType.Contains)]
        public void ParseError2()
        {
          var src = @"{age 120}";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();
        }

        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="ePrematureEOF", MsgMatch=MatchType.Contains)]
        public void ParseError3()
        {
          var src = @"{age: 120";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();
        }


        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eUnterminatedObject", MsgMatch=MatchType.Contains)]
        public void ParseError4()
        {
          var src = @"{age: 120 d";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();

        }

        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eUnterminatedArray", MsgMatch=MatchType.Contains)]
        public void ParseError5()
        {
          var src = @"['age', 120 d";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();

        }

        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eSyntaxError", MsgMatch=MatchType.Contains)]
        public void ParseError6()
        {
          var src = @"[age: 120 d";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();

        }

        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eObjectKeyExpected", MsgMatch=MatchType.Contains)]
        public void ParseError7()
        {
          var src = @"{ true: 120}";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();

        }

        [Run]
        [Aver.Throws(typeof(CodeProcessorException), Message="eDuplicateObjectKey", MsgMatch=MatchType.Contains)]
        public void ParseError8()
        {
          var src = @"{ a: 120, b: 140, a: 12}";

          var parser = new JP(  new JL( new StringSource(src) ), throwErrors: true  );

          parser.Parse();
        }



        [Run]
        public void RootComplexObject()
        {
          var src =
@"
 {FirstName: ""Oleg"",  //comments dont hurt
  'LastName': ""Ogurtsov"",
  ""Middle Name"": 'V.',
  ""Crazy\nName"": 'Shamanov',
  LuckyNumbers: [4,5,6,7,8,9],
  /* comments
  do not break stuff */
  |* in this JSON superset *|
  History:
  [
    #HOT_TOPIC
    {Date: '05/14/1905', What: 'Tsushima'},
    #MODERN_TOPIC
    {Date: '09/01/1939', What: 'WW2 Started', Who: ['Germany','USSR', 'USA', 'Japan', 'Italy', 'Others']}
  ] ,
  Note:
$'This note text
can span many lines
and
this \r\n is not escape'
 }
";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataMap);
          var obj = (JSONDataMap)parser.ResultContext.ResultObject;

          Aver.AreEqual(7, obj.Count);
          Aver.AreObjectsEqual("Oleg", obj["FirstName"]);
          Aver.AreObjectsEqual("Ogurtsov", obj["LastName"]);
          Aver.AreObjectsEqual("V.", obj["Middle Name"]);
          Aver.AreObjectsEqual("Shamanov", obj["Crazy\nName"]);

          var lucky = obj["LuckyNumbers"] as JSONDataArray;
          Aver.IsNotNull(lucky);
          Aver.AreEqual(6, lucky.Count);
          Aver.AreObjectsEqual(4, lucky[0]);
          Aver.AreObjectsEqual(9, lucky[5]);

          var history = obj["History"] as JSONDataArray;
          Aver.IsNotNull(history);
          Aver.AreEqual(2, history.Count);

          var ww2 = history[1] as JSONDataMap;
          Aver.IsNotNull(ww2);
          Aver.AreEqual(3, ww2.Count);

          var who = ww2["Who"] as JSONDataArray;
          Aver.IsNotNull(who);
          Aver.AreEqual(6, who.Count);
          Aver.AreObjectsEqual("USA", who[2]);
        }


         [Run]
        public void RootComplexArray()
        {
          var src =
@"[
 {FirstName: ""Oleg"",  //comments dont hurt
  'LastName': ""Ogurtsov"",
  ""Middle Name"": 'V.',
  ""Crazy\nName"": 'Shamanov',
  LuckyNumbers: [4,5,6,7,8,9],
  /* comments
  do not break stuff */
  |* in this JSON superset *|
  History:
  [
    #HOT_TOPIC
    {Date: '05/14/1905', What: 'Tsushima'},
    #MODERN_TOPIC
    {Date: '09/01/1939', What: 'WW2 Started', Who: ['Germany','USSR', 'USA', 'Japan', 'Italy', 'Others']}
  ] ,
  Note:
$'This note text
can span many lines
and
this \r\n is not escape'
 },
 123
]";

          var parser = new JP(  new JL( new StringSource(src) )  );

          parser.Parse();

          Aver.IsTrue(parser.ResultContext.ResultObject is JSONDataArray);
          var arr = (JSONDataArray)parser.ResultContext.ResultObject;
          Aver.AreEqual(2, arr.Count);
          Aver.AreObjectsEqual(123, arr[1]);

          var obj = (JSONDataMap)arr[0];

          Aver.AreEqual(7, obj.Count);
          Aver.AreObjectsEqual("Oleg", obj["FirstName"]);
          Aver.AreObjectsEqual("Ogurtsov", obj["LastName"]);
          Aver.AreObjectsEqual("V.", obj["Middle Name"]);
          Aver.AreObjectsEqual("Shamanov", obj["Crazy\nName"]);

          var lucky = obj["LuckyNumbers"] as JSONDataArray;
          Aver.IsNotNull(lucky);
          Aver.AreEqual(6, lucky.Count);
          Aver.AreObjectsEqual(4, lucky[0]);
          Aver.AreObjectsEqual(9, lucky[5]);

          var history = obj["History"] as JSONDataArray;
          Aver.IsNotNull(history);
          Aver.AreEqual(2, history.Count);

          var ww2 = history[1] as JSONDataMap;
          Aver.IsNotNull(ww2);
          Aver.AreEqual(3, ww2.Count);

          var who = ww2["Who"] as JSONDataArray;
          Aver.IsNotNull(who);
          Aver.AreEqual(6, who.Count);
          Aver.AreObjectsEqual("USA", who[2]);
        }


    }


}
