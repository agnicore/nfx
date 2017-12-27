namespace rsc
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Rsc.ProgramBody.Main(args);
    }
  }
}
