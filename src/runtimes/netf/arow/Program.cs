namespace arow
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Arow.ProgramBody.Main(args);
    }
  }
}
