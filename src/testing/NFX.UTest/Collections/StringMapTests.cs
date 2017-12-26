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
using System.Threading;
using System.Threading.Tasks;

using NFX.Collections;
using NFX.Scripting;

namespace NFX.UTest.Collections
{

  [Runnable(TRUN.BASE, 2)]
  public class StringMapTests
  {
    [Run]
    public void CaseSensitive()
    {
      var m = new StringMap(true);
      m["a"] = "Albert";
      m["A"] = "Albert Capital";

      Aver.AreEqual(2, m.Count);
      Aver.AreEqual("Albert", m["a"]);
      Aver.AreEqual("Albert Capital", m["A"]);
    }

    [Run]
    public void CaseSensitive_dfltCtor()
    {
      var m = new StringMap();
      m["a"] = "Albert";
      m["A"] = "Albert Capital";

      Aver.AreEqual(2, m.Count);
      Aver.AreEqual("Albert", m["a"]);
      Aver.AreEqual("Albert Capital", m["A"]);
    }

    [Run]
    public void CaseInsensitive()
    {
      var m = new StringMap(false);
      m["a"] = "Albert";
      m["A"] = "Albert Capital";

      Aver.AreEqual(1, m.Count);
      Aver.AreEqual("Albert Capital", m["a"]);
      Aver.AreEqual("Albert Capital", m["A"]);
    }

    [Run]
    public void KeyExistence()
    {
      var m = new StringMap();
      m["a"] = "Albert";
      m["b"] = "Benedict";

      Aver.AreEqual(2, m.Count);
      Aver.AreEqual("Albert", m["a"]);
      Aver.AreEqual("Benedict", m["b"]);
      Aver.IsNull(  m["c"] );
      Aver.IsTrue( m.ContainsKey("a"));
      Aver.IsFalse( m.ContainsKey("c"));
    }


  }
}
