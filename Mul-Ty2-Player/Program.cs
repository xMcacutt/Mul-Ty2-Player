
namespace MT2PClient
{
    static class Program
    {
        public static Client Client;

        static void Main(string[] args)
        {
            while (!ProcessHandler.FindTyProcess())
            {

            }
            Console.WriteLine("Found Ty2");
            Console.ReadLine();
            Client = new Client();
            Console.WriteLine("Enter Name");
            string name = Console.ReadLine();
            string pass = "XXXXX";
            Console.WriteLine("Enter IP");
            string ip = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Starting Client");
            Client.StartClient(name, pass, ip);
        }
    }
}