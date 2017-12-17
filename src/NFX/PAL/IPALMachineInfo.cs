using System;
using System.Collections.Generic;
using System.Text;

using NFX.OS;

namespace NFX.PAL
{
  /// <summary>
  /// Provides functions for getting Machine/OS info like CPU and RAM usage
  /// </summary>
  public interface IPALMachineInfo
  {
    bool IsMono { get; }
    int CurrentProcessorUsagePct { get; }
    int CurrentAvailableMemoryMb { get; }

    MemoryStatus GetMemoryStatus();
  }
}
