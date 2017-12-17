namespace ntc
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Ntc.ProgramBody.Main(args);
    }
  }
}
