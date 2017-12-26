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

using NFX.DataAccess;
using NFX.DataAccess.CRUD;
using NFX.Scripting;

namespace NFX.UTest.DataAccess
{
    [Runnable(TRUN.BASE, 5)]
    public class QuerySourceParsing
    {
        [Run]
        public void WithoutPRAGMA_1()
        {
            var src = "abc";

            var qs = new QuerySource("1", src);

            Aver.IsFalse( qs.HasPragma );
            Aver.IsTrue( qs.ReadOnly );
            Aver.AreEqual("abc", qs.OriginalSource);
            Aver.AreEqual("abc", qs.StatementSource);
        }

        [Run]
        public void WithoutPRAGMA_2()
        {
            var src =
@"a123
b
c
d
e
f
g
h
j
k";

            var qs = new QuerySource("1", src);

            Aver.IsFalse( qs.HasPragma );
            Aver.IsTrue( qs.ReadOnly );
            Aver.AreEqual("a123", qs.OriginalSource.ReadLine());
            Aver.AreEqual("a123", qs.StatementSource.ReadLine());
        }


        [Run]
        public void PRAGMA_1_Modifiable()
        {
            var src =
@"#pragma
modify=tbl_patient
key=counter,ssn
ignore=marker
load=counter
@last_name=lname
@first_name=fname
.last_name=This is description of last name
invisible=marker,counter,c_doctor

select
 1 as marker,
 t1.counter,
 t1.ssn,
 t1.lname as last_name,
 t1.fname as first_name,
 t1.c_doctor,
 t2.phone as doctor_phone,
 t2.NPI	as doctor_id
from
 tbl_patient t1
  left outer join tbl_doctor t2 on t1.c_doctor = t2.counter
where
 t1.lname like ?LN";

            var qs = new QuerySource("1", src);

            Aver.IsTrue( qs.HasPragma );
            Aver.IsFalse( qs.ReadOnly );
            Aver.AreEqual("tbl_patient", qs.ModifyTarget);
            Aver.AreEqual(true, qs.ColumnDefs["counter"].Key);
            Aver.AreEqual(true, qs.ColumnDefs["ssn"].Key);
            Aver.AreEqual("lname", qs.ColumnDefs["last_name"].BackendName);
            Aver.AreEqual("fname", qs.ColumnDefs["first_name"].BackendName);
            Aver.AreEqual("This is description of last name", qs.ColumnDefs["last_name"].Description);
            Aver.IsTrue(StoreFlag.OnlyLoad == qs.ColumnDefs["counter"].StoreFlag);
            Aver.IsTrue(StoreFlag.None == qs.ColumnDefs["marker"].StoreFlag);

            Aver.IsTrue(StoreFlag.LoadAndStore == qs.ColumnDefs["ssn"].StoreFlag);

            Aver.IsFalse( qs.ColumnDefs["marker"].Visible );
            Aver.IsFalse( qs.ColumnDefs["counter"].Visible );
            Aver.IsFalse( qs.ColumnDefs["c_doctor"].Visible );
            Aver.IsTrue(  qs.ColumnDefs["ssn"].Visible );
            Aver.IsTrue(  qs.ColumnDefs["last_name"].Visible );


            Aver.AreEqual(
@"select
 1 as marker,
 t1.counter,
 t1.ssn,
 t1.lname as last_name,
 t1.fname as first_name,
 t1.c_doctor,
 t2.phone as doctor_phone,
 t2.NPI	as doctor_id
from
 tbl_patient t1
  left outer join tbl_doctor t2 on t1.c_doctor = t2.counter
where
 t1.lname like ?LN
" , qs.StatementSource);
        }


        [Run]
        public void PRAGMA_2_nonModifiable()
        {
            var src =
@"#pragma
key=counter,ssn
ignore=marker
load=counter
@last_name=lname
@first_name=fname
.last_name=This is description of last name

select
 1 as marker,
 t1.counter,
 t1.ssn,
 t1.lname as last_name,
 t1.fname as first_name,
 t1.c_doctor,
 t2.phone as doctor_phone,
 t2.NPI	as doctor_id
from
 tbl_patient t1
  left outer join tbl_doctor t2 on t1.c_doctor = t2.counter
where
 t1.lname like ?LN";

            var qs = new QuerySource("1", src);

            Aver.IsTrue( qs.HasPragma );
            Aver.IsTrue( qs.ReadOnly );
            Aver.AreEqual(string.Empty, qs.ModifyTarget);
            Aver.AreEqual(true, qs.ColumnDefs["counter"].Key);
            Aver.AreEqual(true, qs.ColumnDefs["ssn"].Key);
            Aver.AreEqual("lname", qs.ColumnDefs["last_name"].BackendName);
            Aver.AreEqual("fname", qs.ColumnDefs["first_name"].BackendName);
            Aver.AreEqual("This is description of last name", qs.ColumnDefs["last_name"].Description);
            Aver.IsTrue(StoreFlag.OnlyLoad == qs.ColumnDefs["counter"].StoreFlag);
            Aver.IsTrue(StoreFlag.None == qs.ColumnDefs["marker"].StoreFlag);

            Aver.IsTrue(StoreFlag.LoadAndStore == qs.ColumnDefs["ssn"].StoreFlag);

            Aver.AreEqual(
@"select
 1 as marker,
 t1.counter,
 t1.ssn,
 t1.lname as last_name,
 t1.fname as first_name,
 t1.c_doctor,
 t2.phone as doctor_phone,
 t2.NPI	as doctor_id
from
 tbl_patient t1
  left outer join tbl_doctor t2 on t1.c_doctor = t2.counter
where
 t1.lname like ?LN
" , qs.StatementSource);
        }


    }
}
