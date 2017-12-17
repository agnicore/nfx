namespace toy
{
  class Program
  {
    static void Main(string[] args)
    {
      NFX.PAL.NetCore20.NetCore20Runtime.Init();
      BusinessLogic.Toy.ProgramBody.Main(args);
    }
  }
}
