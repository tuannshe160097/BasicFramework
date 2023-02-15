using System.Net.Sockets;
using System.Net;
using System.Text;
using BasicFramework.Kernel;
using BasicFramework.Network.Http;
using BasicFramework.DependencyInjection;
using System.Reflection;
using BasicFramework.Network.Tcp;

namespace BasicFramework
{
    public static class App
    {
        public static void StartApplication()
        {
            Console.WriteLine("Setting up application\n");

            RouterKernel.MapRoutes();
            TcpServer.StartServer();
        }
    }
}
