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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFX.Scripting;

namespace NFX.UTest
{
    [Runnable(TRUN.BASE)]
    public class FIDTests
    {
        [Run]
        public void FID1()
        {
            var f = new FID(1);
            var s = f.ToString();
            Console.WriteLine(s);
            Aver.AreEqual("0-0-1", s);
        }

        [Run]
        public void FID2()
        {
            var f = new FID(4353425342532);
            var s = f.ToString();
            Console.WriteLine(s);
            Aver.AreEqual("3-16096351-68", s);
        }

        [Run]
        public void FID3()
        {
            var CNT = 500000;

            for(var c=0; c<100; c++)
            {
                var set1 = new FID[CNT];
                var set2 = new FID[CNT];
                for(var i=0; i<CNT; i++)
                {
                  set1[i] = FID.Generate();
                  set2[i] = FID.Generate();
                }

                Aver.IsFalse( set1.Intersect(set2).Any() );
                Console.WriteLine("{0}: Set of {1} FIDs no intersection", c, CNT);
            }
        }

        [Run("CNT=10000   tCNT= 4   ")]
        [Run("CNT=100000  tCNT= 4   ")]
        [Run("CNT=400000  tCNT= 4   ")]
        [Run("CNT=10000   tCNT= 10  ")]
        [Run("CNT=100000  tCNT= 10  ")]
        [Run("CNT=400000  tCNT= 10  ")]
        [Run("CNT=10000   tCNT= 100 ")]
        [Run("CNT=100000  tCNT= 100 ")]
        [Run("CNT=400000  tCNT= 100 ")]

        //reexecute same test many times
        [Run("CNT=400000  tCNT=101 ")]
        [Run("CNT=400000  tCNT=102 ")]
        [Run("CNT=400000  tCNT=103 ")]
        [Run("CNT=400000  tCNT=104 ")]
        [Run("CNT=400000  tCNT=105 ")]
        [Run("CNT=400000  tCNT=106 ")]
        public void FID4(int CNT, int tCNT)
        {
            var tasks = new List<Task>();
            var sets= new List<FID>();
            var bag = new ConcurrentBag<FID[]>();

            for(var c=0; c<tCNT; c++)
            {
                tasks.Add( Task.Factory.StartNew(()=>
                {
                  var set = new FID[CNT];

                  for(var i=0; i<CNT; i++)
                  {
                    set[i] = FID.Generate();
                  }

                  bag.Add( set );
                }));
            }

            Task.WaitAll( tasks.ToArray() );

            foreach(var set in bag)
             sets.AddRange( set );


            Console.WriteLine("Analyzing {0:n} FIDs", sets.Count);
            Aver.IsTrue( sets.AsParallel().Distinct().Count() == sets.Count() );
            Console.WriteLine("Done. All ok");
        }

    }
}
