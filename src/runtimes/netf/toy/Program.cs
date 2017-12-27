namespace toy
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      BusinessLogic.Toy.ProgramBody.Main(args);
    }
  }
}
