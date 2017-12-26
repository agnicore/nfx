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

using NFX.DataAccess.CRUD;
using NFX.Scripting;
using NFX.Serialization.Slim;

namespace NFX.UTest.DataAccess
{
    [Runnable(TRUN.BASE, 5)]
    public class SchemaEQU
    {

        [Run]
        public void TypedRows()
        {
            var tbl = new Table(Schema.GetForTypedRow(typeof(Person)));


             tbl.Insert( new Person{
                                    ID = "POP1",
                                    FirstName = "Oleg",
                                    LastName = "Popov-1",
                                    DOB = new DateTime(1953, 12, 10),
                                    YearsInSpace = 12
                                   });

            var ser = new SlimSerializer();
            using(var ms = new MemoryStream())
            {
                ser.Serialize(ms, tbl);

                ms.Position = 0;

                var tbl2 = ser.Deserialize(ms) as Table;

                Aver.IsNotNull( tbl2 );
                Aver.IsFalse( object.ReferenceEquals(tbl ,tbl2) );

                Aver.IsFalse( object.ReferenceEquals(tbl.Schema ,tbl2.Schema) );


                Aver.IsTrue( tbl.Schema.IsEquivalentTo(tbl2.Schema));
            }
        }


        [Run]
        public void DynamicRows()
        {
            var tbl = new Table(Schema.GetForTypedRow(typeof(Person)));


                var row = new DynamicRow( tbl.Schema );

                row["ID"] = "POP1";
                row["FirstName"] = "Oleg";
                row["LastName"] = "Popov-1";
                row["DOB"] = new DateTime(1953, 12, 10);
                row["YearsInSpace"] = 12;

                tbl.Insert( row );


            var ser = new SlimSerializer();
            using(var ms = new MemoryStream())
            {
                ser.Serialize(ms, tbl);

                ms.Position = 0;

                var tbl2 = ser.Deserialize(ms) as Table;

                Aver.IsNotNull( tbl2 );
                Aver.IsFalse( object.ReferenceEquals(tbl ,tbl2) );

                Aver.IsFalse( object.ReferenceEquals(tbl.Schema ,tbl2.Schema) );

                Aver.IsTrue( tbl.Schema.IsEquivalentTo(tbl2.Schema));
            }
        }





    }
}
