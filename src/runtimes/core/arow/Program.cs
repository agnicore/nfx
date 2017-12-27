namespace arow
{
    class Program
    {
        static void Main(string[] args)
        {
          new NFX.PAL.NetCore20.NetCore20Runtime();
          NFX.Tools.Arow.ProgramBody.Main(args);
        }
    }
}
