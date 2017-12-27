using System;
using System.Collections.Generic;
using System.Text;

namespace NFX.PAL
{
  /// <summary>
  /// Internal framework class, business app developers should not use this class directly as it provides a lower-level services
  /// to the higher level code like NFX.Graphics.Canvas, Image etc.
  /// PAL provides process-global injection point for PAL.
  /// The injection of a concrete runtime is done at process entry point which is
  /// statically linked against a concrete runtime.
  /// </summary>
  public static class PlatformAbstractionLayer
  {
    private static object s_Lock = new object();
    private static PALImplementation s_Implementation;


    /// <summary>
    /// Returns the name of the platform implementation, for example ".NET 4.7.1"
    /// </summary>
    public static string PlatformName { get => s_Implementation?.Name; }

    /// <summary>
    /// Abstracts functions related to working with graphical images
    /// </summary>
    public static Graphics.IPALGraphics Graphics
    {
      get
      {
        var result = s_Implementation?.Graphics;
        if (result==null) throw new PALException(StringConsts.PAL_ABSTRACTION_IS_NOT_PROVIDED_ERROR.Args("Graphics"));
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
        var result = s_Implementation?.MachineInfo;
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
        var result = s_Implementation?.FileSystem;
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
    /// System method. Developers do not call. Entry point invoked by runtime implementation modules.
    /// </summary>
    public static void ____SetImplementation(PALImplementation implementation)
    {
      if (implementation==null)
        throw new PALException(StringConsts.ARGUMENT_ERROR+"{0}.{1}(implementation=null)".Args(nameof(PlatformAbstractionLayer), nameof(____SetImplementation)));

      var caller = new System.Diagnostics.StackFrame(1, false);
      var mi = caller.GetMethod();
      var callerClass = mi.DeclaringType;
      if (!callerClass.IsSubclassOf(typeof(PALImplementation)))
        throw new PALException(StringConsts.ARGUMENT_ERROR+"{0}.{1}: {2}".Args(nameof(PlatformAbstractionLayer), nameof(____SetImplementation), StringConsts.PAL_IMPLEMENTATION_INJECTION_ERROR));


      lock(s_Lock)
      {
        if (s_Implementation!=null)
          throw new PALException(StringConsts.PAL_ALREADY_SET_ERROR);

        s_Implementation = implementation;
      }
    }
  }
}
