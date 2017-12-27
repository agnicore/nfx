namespace trun
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Trun.ProgramBody.Main(args);
    }
  }
}
