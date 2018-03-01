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
using System.IO;

using NFX.Scripting;

using NFX.Serialization.JSON;

namespace NFX.UTest
{
  [Runnable(TRUN.BASE)]
  public class BaseExceptionsTests
  {

    [Run]
    public void WrappedExceptionData_BSON()
    {
      try
      {
        throw new NFXException("Oy vei!", new NFXException("Inside")){ Code = 223322, Source = "Karlson" };
      }
      catch(Exception caught)
      {
        var wed = new WrappedExceptionData(caught);
        var ser = new NFX.Serialization.BSON.BSONSerializer();

        var doc = ser.Serialize(wed);

        var wed2 = new WrappedExceptionData();
        object ctx = null;
        wed2.DeserializeFromBSON(ser, doc, ref ctx);

        Console.WriteLine();
        Console.WriteLine($"BSON:");
        Console.WriteLine($"-----------------------------");
        Console.WriteLine(doc.ToJSON());

        averWrappedExceptionEquality(wed, wed2);
      }
    }

    [Run]
    public void WrappedExceptionData_Slim()
    {
      try
      {
        throw new NFXException("Oy vei!", new NFXException("Inside")){ Code = 223322, Source = "Karlson" };
      }
      catch(Exception caught)
      {
        var wed = new WrappedExceptionData(caught);
        var ser = new NFX.Serialization.Slim.SlimSerializer();

        using(var ms = new MemoryStream())
        {
          ser.Serialize(ms, wed);
          ms.Position = 0;

          var bin = ms.ToArray();
          Console.WriteLine();
          Console.WriteLine($"Bin {bin.Length} bytes:");
          Console.WriteLine($"-----------------------------");
          Console.WriteLine(bin.ToDumpString(DumpFormat.Hex));

          var wed2 = ser.Deserialize(ms) as WrappedExceptionData;
          averWrappedExceptionEquality(wed, wed2);
        }
      }
    }


    [Run]
    public void WrappedExceptionData_BASE64()
    {
      try
      {
        throw new NFXException("Oy vei!", new NFXException("Inside")){ Code = 223322, Source = "Karlson" };
      }
      catch(Exception caught)
      {
        var wed = new WrappedExceptionData(caught);
        var base64 = wed.ToBase64();

        Console.WriteLine();
        Console.WriteLine($"Base64 {base64.Length} bytes:");
        Console.WriteLine($"-----------------------------");
        Console.WriteLine(base64);

        var wed2 = WrappedExceptionData.FromBase64(base64);
        averWrappedExceptionEquality(wed, wed2);
      }
    }

    private void averWrappedExceptionEquality(WrappedExceptionData d1, WrappedExceptionData d2)
    {
      Aver.IsNotNull(d1);
      Aver.IsNotNull(d2);
      Aver.AreNotSameRef(d1, d2);

      Aver.AreEqual(d1.Message, d2.Message);
      Aver.AreEqual(d1.Code, d2.Code);
      Aver.AreEqual(d1.Source, d2.Source);
      Aver.AreEqual(d1.TypeName, d2.TypeName);
      Aver.AreEqual(d1.ApplicationName, d2.ApplicationName);
      Aver.AreEqual(d1.StackTrace, d2.StackTrace);
      Aver.AreEqual(d1.WrappedData, d2.WrappedData);

      if (d1.InnerException!=null)
        averWrappedExceptionEquality(d1.InnerException, d2.InnerException);
    }

  }
}
