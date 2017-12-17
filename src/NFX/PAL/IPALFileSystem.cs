using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace NFX.PAL
{
  /// <summary>
  /// Provides functions for working with file system
  /// </summary>
  public interface IPALFileSystem
  {
    /// <summary>
    /// Creates directory and immediately grants it accessibility rules for everyone if it does not exists,
    ///  or returns the existing directory
    /// </summary>
    DirectoryInfo EnsureAccessibleDirectory(string path);
  }
}
