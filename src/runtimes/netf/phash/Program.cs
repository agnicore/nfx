namespace phash
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Phash.ProgramBody.Main(args);
    }
  }
}
