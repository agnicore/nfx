namespace licupd
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Licupd.ProgramBody.Main(args);
    }
  }
}
