namespace inventory
{
    class Program
    {
        static void Main(string[] args)
        {
          NFX.PAL.NetCore20.NetCore20Runtime.Init();
          NFX.Tools.Inventory.ProgramBody.Main(args);
        }
    }
}
