namespace getdatetime
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Getdatetime.ProgramBody.Main(args);
    }
  }
}
