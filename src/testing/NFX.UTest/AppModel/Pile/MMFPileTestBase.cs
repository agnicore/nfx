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

using NFX;
using NFX.ApplicationModel.Pile;
using NFX.Scripting;

namespace NFX.UTest.AppModel.Pile
{
  public class MMFPileTestBase : IRunHook
  {
      public const string LOCAL_ROOT = @"c:\NFX\ut-pile";


      bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
      {
        GC.Collect();
        Directory.CreateDirectory(LOCAL_ROOT);
        return false;
      }

      bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
      {
        GC.Collect();
        NFX.IOMiscUtils.EnsureDirectoryDeleted(LOCAL_ROOT);
        return false;
      }


      public MMFPile MakeMMFPile()
      {
        var result = new MMFPile("UT");
        result.DataDirectoryRoot = LOCAL_ROOT;
        return result;
      }
  }
}
