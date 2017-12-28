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
using NFX.Scripting;

namespace NFX.ITest.AppModel.Pile
{
  [Runnable]
  public class PileFragmentationTest64Gb : HighMemoryLoadTest64RAM
  {
    [Run("cnt=100000  durationSec=30  speed=true   payloadSizeMin=1000  payloadSizeMax=16000  deleteFreq=3   isParallel=true")]
    [Run("cnt=100000  durationSec=30  speed=false  payloadSizeMin=1000  payloadSizeMax=16000  deleteFreq=10  isParallel=true")]
    public void Put_RandomDelete_ByteArray(int cnt, int durationSec, bool speed, int payloadSizeMin, int payloadSizeMax, int deleteFreq, bool isParallel)
    {
      PileFragmentationTest.Put_RandomDelete_ByteArray(cnt, durationSec, speed, payloadSizeMin, payloadSizeMax, deleteFreq, isParallel);
    }

    [Run("speed=true   durationSec=30  payloadSizeMin=1000  payloadSizeMax=16000  deleteFreq=3  isParallel=true")]
    [Run("speed=false  durationSec=30  payloadSizeMin=1000  payloadSizeMax=16000  deleteFreq=3  isParallel=true")]
    public void DeleteOne_ByteArray(bool speed, int durationSec, int payloadSizeMin, int payloadSizeMax, int deleteFreq, bool isParallel)
    {
      PileFragmentationTest.DeleteOne_ByteArray(speed, durationSec, payloadSizeMin, payloadSizeMax, deleteFreq, isParallel);
    }

    [Run("speed=true   durationSec=30  payloadSizeMin=1000  payloadSizeMax=16000  isParallel=true")]
    [Run("speed=false  durationSec=30  payloadSizeMin=1000  payloadSizeMax=16000  isParallel=true")]
    public void Chessboard_ByteArray(bool speed, int durationSec, int payloadSizeMin, int payloadSizeMax, bool isParallel)
    {
      PileFragmentationTest.Chessboard_ByteArray(speed, durationSec, payloadSizeMin, payloadSizeMax, isParallel);
    }

    [Run("speed=true   durationSec=30  putMin=100  putMax=200  delFactor=4  payloadSizeMin=1000  payloadSizeMax=16000  isParallel=true")]
    [Run("speed=false  durationSec=30  putMin=100  putMax=200  delFactor=4  payloadSizeMin=1000  payloadSizeMax=16000  isParallel=true")]
    public void DeleteSeveral_ByteArray(bool speed, int durationSec, int putMin, int putMax, int delFactor, int payloadSizeMin, int payloadSizeMax, bool isParallel)
    {
      PileFragmentationTest.DeleteSeveral_ByteArray(speed, durationSec, putMin, putMax, delFactor, payloadSizeMin, payloadSizeMax, isParallel);
    }

    [Run("speed=true   durationSec=30  payloadSizeMin=2  payloadSizeMax=16000  countMin=100   countMax=2000")]
    [Run("speed=false  durationSec=30  payloadSizeMin=2  payloadSizeMax=16000  countMin=100   countMax=2000")]
    [Run("speed=true   durationSec=30  payloadSizeMin=2  payloadSizeMax=16000  countMin=1000  countMax=2000")]
    [Run("speed=false  durationSec=30  payloadSizeMin=2  payloadSizeMax=16000  countMin=1000  countMax=2000")]

    [Run("speed=true   durationSec=30  payloadSizeMin=90333  payloadSizeMax=160333  countMin=100   countMax=2000")]
    [Run("speed=false  durationSec=30  payloadSizeMin=90333  payloadSizeMax=160333  countMin=100   countMax=2000")]
    [Run("speed=true   durationSec=30  payloadSizeMin=90333  payloadSizeMax=160333  countMin=1000  countMax=2000")]
    [Run("speed=false  durationSec=30  payloadSizeMin=90333  payloadSizeMax=160333  countMin=1000  countMax=2000")]
    public void NoGrowth_ByteArray(bool speed, int durationSec, int payloadSizeMin, int payloadSizeMax, int countMin, int countMax)
    {
      PileFragmentationTest.NoGrowth_ByteArray(speed, durationSec, payloadSizeMin, payloadSizeMax, countMin, countMax);
    }
  }
}
