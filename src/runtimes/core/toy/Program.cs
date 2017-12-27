namespace toy
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetCore20.NetCore20Runtime();
      BusinessLogic.Toy.ProgramBody.Main(args);
    }
  }
}
