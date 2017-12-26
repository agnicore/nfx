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

using NFX.DataAccess.CRUD;
using NFX.Scripting;
using static NFX.Aver.ThrowsAttribute;

namespace NFX.UTest.DataAccess
{
    [Runnable(TRUN.BASE, 5)]
    public class FieldAttrCloningTests
    {
        [Run]
        public void T1()
        {
            var schema1 = Schema.GetForTypedRow<Row1>();
            Aver.AreEqual(3, schema1.FieldCount);

            Aver.IsTrue( schema1["First_Name"][null].Required );
            Aver.AreEqual(45, schema1["First_Name"][null].MaxLength);

            Aver.IsTrue( schema1["Last_Name"][null].Required );
            Aver.AreEqual(75, schema1["Last_Name"][null].MaxLength);

            Aver.IsFalse( schema1["Age"][null].Required );
            Aver.AreObjectsEqual(150, schema1["Age"][null].Max);




            var schema2 = Schema.GetForTypedRow<Row2>();
            Aver.AreEqual(3, schema2.FieldCount);

            Aver.IsTrue( schema2["First_Name"][null].Required );
            Aver.AreEqual(47, schema2["First_Name"][null].MaxLength);

            Aver.IsTrue( schema2["Last_Name"][null].Required );
            Aver.AreEqual(75, schema2["Last_Name"][null].MaxLength);

            Aver.IsFalse( schema2["Age"][null].Required );
            Aver.AreObjectsEqual(150, schema2["Age"][null].Max);
        }

        [Run]
        [Aver.Throws(typeof(CRUDException), Message="only a single", MsgMatch=MatchType.Contains)]
        public void T2()
        {
            var schema1 = Schema.GetForTypedRow<Row3>();
        }

        [Run]
        [Aver.Throws(typeof(CRUDException), Message="there is no field", MsgMatch=MatchType.Contains)]
        public void T3()
        {
            var schema1 = Schema.GetForTypedRow<Row4>();
        }

        [Run]
        [Aver.Throws(typeof(CRUDException), Message="recursive field", MsgMatch=MatchType.Contains)]
        public void T4()
        {
            var schema1 = Schema.GetForTypedRow<Row5>();
        }

        [Run]
        [Aver.Throws(typeof(CRUDException), Message="recursive field", MsgMatch=MatchType.Contains)]
        public void T5()
        {
            var schema1 = Schema.GetForTypedRow<Row6>();
        }

        [Run]
        [Aver.Throws(typeof(CRUDException), Message="recursive field", MsgMatch=MatchType.Contains)]
        public void T6()
        {
            var schema1 = Schema.GetForTypedRow<Row7>();
        }


        public class Row1 : TypedRow
        {
          [Field(required: true, maxLength: 45)]
          public string First_Name{ get; set;}

          [Field(required: true, maxLength: 75)]
          public string Last_Name{ get; set;}

          [Field(required: false, max: 150)]
          public int? Age{ get; set;}
        }

        public class Row2 : TypedRow
        {
          [Field(typeof(Row1), "First_Name", maxLength: 47)]
          public string First_Name{ get; set;}

          [Field(typeof(Row1))]
          public string Last_Name{ get; set;}

          [Field(typeof(Row1))]
          public int? Age{ get; set;}
        }

        public class Row3 : TypedRow
        {
          [Field(typeof(Row1))]
          [Field] //cant have mnore than one
          public string Last_Name{ get; set;}
        }

        public class Row4 : TypedRow
        {
          [Field(typeof(Row1))]
          public string DOESNOTEXIST{ get; set;}
        }

        public class Row5 : TypedRow
        {
          [Field(required: true, maxLength: 45)]
          public string First_Name{ get; set;}

          [Field(required: true, maxLength: 75)]
          public string Last_Name{ get; set;}

          [Field(typeof(Row7))]
          public int? Age{ get; set;}
        }

        public class Row6 : TypedRow
        {
          [Field(typeof(Row5), "First_Name", maxLength: 47)]
          public string First_Name{ get; set;}

          [Field(typeof(Row5))]
          public string Last_Name{ get; set;}

          [Field(typeof(Row5))]
          public int? Age{ get; set;}
        }

         public class Row7 : TypedRow
        {
          [Field(typeof(Row6))]
          public int? Age{ get; set;}
        }

    }
}
