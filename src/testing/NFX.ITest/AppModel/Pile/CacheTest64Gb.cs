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

using NFX.UTest.AppModel.Pile;
using NFX.Scripting;

namespace NFX.ITest.AppModel.Pile
{
    [Runnable]
    public class CacheTest64gb : HighMemoryLoadTest64RAM
    {
        [Run("cnt=10000000  tbls=1")]
        [Run("cnt=10000000  tbls=16")]
        [Run("cnt=10000000  tbls=512")]
        public void T190_FID_PutGetCorrectness(int cnt, int tbls)
        {
            PileCacheTestCore.FID_PutGetCorrectness(cnt, tbls);
        }

        [Run("workers=16  tables=7   putCount=25000   durationSec=40")]
        [Run("workers=5   tables=20  putCount=50000   durationSec=20")]
        [Run("workers=16  tables=20  putCount=150000  durationSec=40")]
        public void T9000000_ParalellGetPutRemove(int workers, int tables, int putCount, int durationSec)
        {
            PileCacheTestCore.ParalellGetPutRemove(workers, tables, putCount, durationSec);
        }
    }
}
