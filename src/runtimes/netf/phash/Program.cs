namespace phash
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Phash.ProgramBody.Main(args);
    }
  }
}
