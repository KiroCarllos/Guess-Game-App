using Guss_Name_Server_App;
using System.Threading;
namespace New_Server
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }


    }
}
