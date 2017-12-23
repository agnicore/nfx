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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using NFX.Scripting;

using NFX;
using NFX.ApplicationModel.Pile;

namespace NFX.UTest.AppModel.Pile
{
  [Runnable]
  public class PileEnumerationTest : IRunHook
  {
      bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
      {
        GC.Collect();
        return false;
      }

      bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
      {
        GC.Collect();
        return false;
      }


      [Run("count=723     segmentSize=67108864")]//count < 1024
      [Run("count=1500    segmentSize=67108864")]//1 segment
      [Run("count=250000  segmentSize=67108864")]
      [Run("count=750000  segmentSize=67108864")]
      public void Buffers(int count, int segmentSize)
      {
        using (var pile = new DefaultPile())
        {
          pile.SegmentSize = segmentSize;
          pile.Start();

          var hs = new HashSet<int>();

          for(var i=0; i<count; i++)
          {
            var buf = new byte[4 + (i%512)];
            buf.WriteBEInt32(i);
            pile.Put(buf);
            hs.Add(i);
          }

          Console.WriteLine("Created {0} segments".Args(pile.SegmentCount));

          var j = 0;
          foreach(var entry in pile)
          {
            var buf = pile.Get(entry.Pointer) as byte[];
            Aver.IsNotNull(buf);
            Aver.IsTrue(buf.Length>=4);
            var i = buf.ReadBEInt32();
            Aver.IsTrue(hs.Remove(i));
            Aver.IsTrue( entry.Type == PileEntry.DataType.Buffer );
            j++;
          }
          Aver.AreEqual(j, count);
          Aver.AreEqual(0, hs.Count);
        }
      }
  }
}
