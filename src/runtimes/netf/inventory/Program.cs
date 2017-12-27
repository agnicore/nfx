namespace inventory
{
  class Program
  {
    static void Main(string[] args)
    {
      new NFX.PAL.NetFramework.DotNetFrameworkRuntime();
      NFX.Tools.Inventory.ProgramBody.Main(args);
    }
  }
}
