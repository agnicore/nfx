namespace gluec
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      NFX.Tools.Gluec.ProgramBody.Main(args);
    }
  }
}
