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
using System.Threading;
using System.Threading.Tasks;


namespace NFX.Wave
{
  public partial class WaveServer
  {
    /// <summary>
    /// Framework internal, app developers do not use.
    /// Provides access to internal members of WaveServer which is need for listeners
    /// and other low-level code.
    /// </summary>
    public struct serverAccessor
    {
      public serverAccessor(WaveServer self) => Self = self;

      public readonly WaveServer Self;


      public void Stat_IncServerRequest()
      {
        if (Self.m_InstrumentationEnabled)
          Interlocked.Increment(ref Self.m_stat_ServerRequest);
      }

      public void Stat_IncServerGateDenial()
      {
        if (Self.m_InstrumentationEnabled)
          Interlocked.Increment(ref Self.m_stat_ServerGateDenial);
      }

      public void Stat_ServerAcceptSemaphoreCount(int acceptCount)
      {
        if (Self.m_InstrumentationEnabled)
          Thread.VolatileWrite(ref Self.m_stat_ServerAcceptSemaphoreCount, acceptCount);
      }

      public void Stat_ServerWorkSemaphoreCount(int workCount)
      {
        if (Self.m_InstrumentationEnabled)
          Thread.VolatileWrite(ref Self.m_stat_ServerWorkSemaphoreCount, workCount);
      }

      public Semaphore WorkSemaphore => Self.m_WorkSemaphore;

      public Task AcceptRequest(Server.IHttpContext context,
                                bool onThisThread) => Self.AcceptRequest(context, onThisThread);
    }

  }

}
