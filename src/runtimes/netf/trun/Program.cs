namespace trun
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Trun.ProgramBody.Main(args);
    }
  }
}
