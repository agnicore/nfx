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

using NFX.PAL;

namespace NFX.OS
{
  /// <summary>
  /// Denotes primary OS Families: Win/Mac/Lin*nix
  /// </summary>
  public enum OSFamily
  {
    Undetermined = 0,
    Windows = 1,
    Linux = 100,
    Mac = 200
  }

  /// <summary>
  /// Provides current memory status snapshot
  /// </summary>
  [Serializable]
  public struct MemoryStatus
  {
    public uint LoadPct { get;  set; }

    public ulong TotalPhysicalBytes { get;  set; }
    public ulong AvailablePhysicalBytes { get;  set; }

    public ulong TotalPageFileBytes { get;  set; }
    public ulong AvailablePageFileBytes { get;  set; }

    public ulong TotalVirtBytes { get;  set; }
    public ulong AvailableVirtBytes { get;  set; }
  }


  /// <summary>
  /// Facilitates various computer-related tasks such as CPU usage, memory utilization etc.
  /// </summary>
  public static class Computer
  {

    static Computer()
    {
    }

    /// <summary>
    /// Returns current computer-wide CPU utilization percentage
    /// </summary>
    public static int CurrentProcessorUsagePct
    {
      get
      {
        return PlatformAbstractionLayer.MachineInfo.CurrentProcessorUsagePct;
      }
    }


    /// <summary>
    /// Returns current computer-wide RAM availability in mbytes
    /// </summary>
    public static int CurrentAvailableMemoryMb
    {
      get
      {
        return PlatformAbstractionLayer.MachineInfo.CurrentAvailableMemoryMb;
      }
    }


    private static OSFamily s_OSFamily;

    /// <summary>
    /// Rsturns OS family for this computer: Linux vs Win vs Mac
    /// </summary>
    public static OSFamily OSFamily
    {
      get
      {
        if (s_OSFamily != OSFamily.Undetermined) return s_OSFamily;

        switch (System.Environment.OSVersion.Platform)
        {
          case PlatformID.Unix:
            {
              // Need to check for Mac-specific root folders, because Mac may get reported as UNIX
              if (System.IO.Directory.Exists("/Applications")
                  && System.IO.Directory.Exists("/System")
                  && System.IO.Directory.Exists("/Users")
                  && System.IO.Directory.Exists("/Volumes"))
              {
                s_OSFamily = OSFamily.Mac;
                break;
              }
              else
              {
                s_OSFamily = OSFamily.Linux;
                break;
              }
            }

          case PlatformID.MacOSX: { s_OSFamily = OSFamily.Mac; break; }

          default: { s_OSFamily = OSFamily.Windows; break; };
        }

        return s_OSFamily;
      }

    }


    private static string s_UniqueNetworkSignature;

    /// <summary>
    /// Returns network signature for this machine which is unique in the eclosing network segment (MAC-based)
    /// </summary>
    public static string UniqueNetworkSignature
    {
      get
      {
        if (s_UniqueNetworkSignature == null)
          s_UniqueNetworkSignature = NetworkUtils.GetMachineUniqueMACSignature();

        return s_UniqueNetworkSignature;
      }
    }

    public static bool IsMono { get { return PlatformAbstractionLayer.MachineInfo.IsMono; } }


    public static MemoryStatus GetMemoryStatus()
    {
      return PlatformAbstractionLayer.MachineInfo.GetMemoryStatus();
    }

  }
}