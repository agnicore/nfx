namespace arow
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Arow.ProgramBody.Main(args);
    }
  }
}
