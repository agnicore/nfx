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

using NFX.Collections;
using NFX.Scripting;

namespace NFX.UTest.Collections
{
    [Runnable]
    public class EventedDictionaryTests
    {

        [Run]
        public void Dict_Readonly()
        {
          var dict = new EventedDictionary<int, string, string>("CONTEXT", false);

          var ro = false;

          dict.GetReadOnlyEvent = (l) => ro;

          dict.Add(1,"a");
          dict.Add(2,"b");
          dict.Add(3,"c");

          Aver.AreEqual(3, dict.Count);
          ro = true;

          Aver.Throws<NFXException>(() =>  dict.Add(4, "d"));
        }

        [Run]
        public void Dict_Add()
        {
          var dict = new EventedDictionary<int, string, string>("CONTEXT", false);

          var first = true;
          dict.GetReadOnlyEvent = (_) => false;

          dict.ChangeEvent = (d, ct, p, k, v) =>
                            {
                              Aver.AreObjectsEqual( EventedDictionary<int, string, string>.ChangeType.Add, ct);
                              Aver.IsTrue( (first ? EventPhase.Before : EventPhase.After) == p);
                              Aver.AreEqual( 1, k);
                              Aver.AreEqual( "a", v);
                              first = false;
                            };

          dict.Add(1, "a");

        }

        [Run]
        public void Dict_Remove()
        {
          var dict = new EventedDictionary<int,string, string>("CONTEXT", false);

          var first = true;
          dict.GetReadOnlyEvent = (_) => false;

          dict.Add(1, "a");
          dict.Add(2, "b");
          dict.Add(3, "c");

          Aver.AreEqual(3, dict.Count);

          dict.ChangeEvent = (d, ct, p, k, v) =>
                            {
                              Aver.AreObjectsEqual( EventedDictionary<int, string, string>.ChangeType.Remove, ct);
                              Aver.IsTrue( (first ? EventPhase.Before : EventPhase.After) == p);
                              Aver.AreEqual( 2, k);
                              first = false;
                            };

          dict.Remove(2);
          Aver.AreEqual(2, dict.Count);

        }

        [Run]
        public void Dict_Set()
        {
          var dict = new EventedDictionary<int, string, string>("CONTEXT", false);

          var first = true;
          dict.GetReadOnlyEvent = (_) => false;

          dict.Add(1, "a");
          dict.Add(2, "b");
          dict.Add(3, "c");

          Aver.AreEqual(3, dict.Count);

          dict.ChangeEvent = (d, ct, p, k, v) =>
                            {
                              Aver.AreObjectsEqual( EventedDictionary<int, string, string>.ChangeType.Set, ct);
                              Aver.IsTrue( (first ? EventPhase.Before : EventPhase.After) == p);
                              Aver.AreEqual( 1, k);
                              Aver.AreEqual( "z", v);
                              first = false;
                            };

          dict[1] = "z";
          Aver.AreEqual("z", dict[1]);

        }


        [Run]
        public void Dict_Clear()
        {
          var dict = new EventedDictionary<int, string, string>("CONTEXT", false);

          var first = true;
          dict.GetReadOnlyEvent = (_) => false;

          dict.Add(1, "a");
          dict.Add(2, "b");
          dict.Add(3, "c");

          Aver.AreEqual(3, dict.Count);

          dict.ChangeEvent = (d, ct, p, k, v) =>
                            {
                              Aver.AreObjectsEqual( EventedDictionary<int, string, string>.ChangeType.Clear, ct);
                              Aver.IsTrue( (first ? EventPhase.Before : EventPhase.After) == p);
                              Aver.AreEqual( 0, k);
                              Aver.AreEqual( null, v);
                              first = false;
                            };

          dict.Clear();
          Aver.AreEqual( 0, dict.Count);

        }



  }
}
