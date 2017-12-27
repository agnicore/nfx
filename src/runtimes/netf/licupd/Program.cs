namespace licupd
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Licupd.ProgramBody.Main(args);
    }
  }
}
