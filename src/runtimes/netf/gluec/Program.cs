namespace gluec
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Gluec.ProgramBody.Main(args);
    }
  }
}
