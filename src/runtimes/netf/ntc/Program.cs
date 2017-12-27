namespace ntc
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Ntc.ProgramBody.Main(args);
    }
  }
}
