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
using System.Text;

using NFX.Collections;
using NFX.Scripting;

namespace NFX.UTest.Collections
{
  [Runnable]
  public class LookAheadEnumerableTest
  {
    [Run]
    public void EmptyEnumerable()
    {
      var enumerator = string.Empty.AsLookAheadEnumerable().GetLookAheadEnumerator();
      Aver.IsFalse(enumerator.HasNext);
      Aver.Throws<NFXException>(() => {
        var next = enumerator.Next;
      }, StringConsts.OPERATION_NOT_SUPPORTED_ERROR + "{0}.Next(!HasNext)".Args(typeof(LookAheadEnumerator<char>).FullName));
      Aver.Throws<InvalidOperationException>(() => {
        var current = enumerator.Current;
      });
      Aver.IsFalse(enumerator.MoveNext());
    }

    [Run]
    public void SingleEnumerable()
    {
      var enumerator = " ".AsLookAheadEnumerable().GetLookAheadEnumerator();
      Aver.IsTrue(enumerator.HasNext);
      Aver.AreEqual(enumerator.Next, ' ');
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
      Aver.IsFalse(enumerator.HasNext);
      Aver.Throws<NFXException>(() =>
      {
        var next = enumerator.Next;
      }, StringConsts.OPERATION_NOT_SUPPORTED_ERROR + "{0}.Next(!HasNext)".Args(typeof(LookAheadEnumerator<char>).FullName));
      Aver.AreEqual(enumerator.Current, ' ');
      Aver.IsFalse(enumerator.MoveNext());
      enumerator.Reset();
      Aver.AreEqual(enumerator.HasNext, true);
      Aver.AreEqual(enumerator.Next, ' ');
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
    }

    [Run]
    public void MulripleEnumerable()
    {
      var enumerator = "+-".AsLookAheadEnumerable().GetLookAheadEnumerator();
      Aver.IsTrue(enumerator.HasNext);
      Aver.AreEqual(enumerator.Next, '+');
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
      Aver.IsTrue(enumerator.HasNext);
      Aver.AreEqual(enumerator.Next, '-');
      Aver.AreEqual(enumerator.Current, '+');
      Aver.IsTrue(enumerator.MoveNext());
      Aver.IsFalse(enumerator.HasNext);
      Aver.Throws<NFXException>(() =>
      {
        var next = enumerator.Next;
      }, StringConsts.OPERATION_NOT_SUPPORTED_ERROR + "{0}.Next(!HasNext)".Args(typeof(LookAheadEnumerator<char>).FullName));
      Aver.AreEqual(enumerator.Current, '-');
      Aver.IsFalse(enumerator.MoveNext());
      enumerator.Reset();
      Aver.IsTrue(enumerator.HasNext);
      Aver.AreEqual(enumerator.Next, '+');
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
    }

    [Run]
    public void EmptyEnumerable_AsEnumerable()
    {
      var enumerator = string.Empty.AsLookAheadEnumerable().GetEnumerator();
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsFalse(enumerator.MoveNext());
    }

    [Run]
    public void SingleEnumerable_AsEnumerable()
    {
      var enumerator = " ".AsLookAheadEnumerable().GetEnumerator();
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
      Aver.AreEqual(enumerator.Current, ' ');
      Aver.IsFalse(enumerator.MoveNext());
      enumerator.Reset();
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
    }

    [Run]
    public void MultipleEnumerable_AsEnumerable()
    {
      var enumerator = "+-".AsLookAheadEnumerable().GetEnumerator();
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
      Aver.AreEqual(enumerator.Current, '+');
      Aver.IsTrue(enumerator.MoveNext());
      Aver.AreEqual(enumerator.Current, '-');
      Aver.IsFalse(enumerator.MoveNext());
      enumerator.Reset();
      Aver.Throws<InvalidOperationException>(() =>
      {
        var current = enumerator.Current;
      });
      Aver.IsTrue(enumerator.MoveNext());
    }

    [Run]
    public void ForEach()
    {
      var sb = new StringBuilder();
      foreach (var c in string.Empty.AsLookAheadEnumerable()) sb.Append(c);
      Aver.AreEqual(sb.ToString(), string.Empty);
      sb.Clear();
      foreach (var c in "+".AsLookAheadEnumerable()) sb.Append(c);
      Aver.AreEqual(sb.ToString(), "+");
      sb.Clear();
      foreach (var c in "+-".AsLookAheadEnumerable()) sb.Append(c);
      Aver.AreEqual(sb.ToString(), "+-");
    }

    [Run]
    public void DetectSimbolPair()
    {
      var enumerator = @" """" ".AsLookAheadEnumerable().GetLookAheadEnumerator();
      var detect = false;
      while (!detect && enumerator.MoveNext())
      {
        if ('\"' == enumerator.Current && enumerator.HasNext && '\"' == enumerator.Next)
          detect = true;
      }
      Aver.IsTrue(detect);
      enumerator = @"""  """.AsLookAheadEnumerable().GetLookAheadEnumerator();
      detect = false;
      while (!detect && enumerator.MoveNext())
      {
        if ('\"' == enumerator.Current && enumerator.HasNext && '\"' == enumerator.Next)
          detect = true;
      }
      Aver.IsFalse(detect);
    }
  }
}
