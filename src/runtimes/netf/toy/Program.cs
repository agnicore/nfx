namespace toy
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetFramework.DotNetFrameworkRuntime.Init();
      BusinessLogic.Toy.ProgramBody.Main(args);
    }
  }
}
