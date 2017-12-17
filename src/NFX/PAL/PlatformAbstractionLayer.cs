using System;
using System.Collections.Generic;
using System.Text;

namespace NFX.PAL
{
  /// <summary>
  /// Internal framework class, business app developers should not use this class directly as it provides a lower-level services
  /// to the higher level code like NFX.Graphics.Canvas, Image etc.
  /// PAL provides process-global injection point for PAL.
  /// The injection of a concrete runtime is done by process entry point which is
  /// statically linked against a concrete runtime.
  /// </summary>
  public static class PlatformAbstractionLayer
  {
    private static object s_Lock = new object();
    private static string s_PlatformName;

    private static IPALImaging s_Imaging;
    private static IPALMachineInfo s_MachineInfo;
    private static IPALFileSystem s_FileSystem;


    public static string PlatformName { get => s_PlatformName; }

    /// <summary>
    /// Abstracts functions related to working with graphical images
    /// </summary>
    public static IPALImaging Imaging
    {
      get
      {
        var result = s_Imaging;
        if (result==null) throw new PALException(StringConsts.PAL_ABSTRACTION_IS_NOT_PROVIDED_ERROR.Args("Imaging"));
        return result;
      }
    }

    /// <summary>
    /// Abstracts functions related to obtaining the machine parameters such as CPU and RAM
    /// </summary>
    public static IPALMachineInfo MachineInfo
    {
      get
      {
        var result = s_MachineInfo;
        if (result==null) throw new PALException(StringConsts.PAL_ABSTRACTION_IS_NOT_PROVIDED_ERROR.Args("MachineInfo"));
        return result;
      }
    }

    /// <summary>
    /// Abstracts functions related to working with file system
    /// </summary>
    public static IPALFileSystem FileSystem
    {
      get
      {
        var result = s_FileSystem;
        if (result==null) throw new PALException(StringConsts.PAL_ABSTRACTION_IS_NOT_PROVIDED_ERROR.Args("FileSystem"));
        return result;
      }
    }


    /// <summary>
    /// Sets invariant culture for the whole process regardless of machine's culture
    /// </summary>
    public static void SetProcessInvariantCulture()
    {
      //For now it looks like this behavior does not need to be abstracted away into implementation layer,
      //this may change in future, t4 this is in PAL
      System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
    }


    /// <summary>
    /// System method. Developers do not call. Entry point invoked by runtime modules.
    /// </summary>
    public static void ____SetImplementation(
                            string platformName,
                            IPALImaging imaging,
                            IPALMachineInfo machine,
                            IPALFileSystem fs)
    {
      if (platformName.IsNullOrWhiteSpace())
        throw new PALException(StringConsts.ARGUMENT_ERROR+"{0}.{1}(platformName=null|empty)".Args(nameof(PlatformAbstractionLayer), nameof(____SetImplementation)));

      lock(s_Lock)
      {
        if (s_PlatformName!=null)
          throw new PALException(StringConsts.PAL_ALREADY_SET_ERROR);

        s_PlatformName = platformName;
        s_Imaging = imaging;
        s_MachineInfo = machine;
        s_FileSystem = fs;
      }
    }
  }
}
