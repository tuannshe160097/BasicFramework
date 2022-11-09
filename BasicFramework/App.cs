using System.Net.Sockets;
using System.Net;
using System.Text;
using BasicFramework.Kernel;
using BasicFramework.Http;
using BasicFramework.DependencyInjection;

namespace BasicFramework
{
    public static class App
    {
        public static void StartApplication(string host, int port)
        {
            Console.WriteLine("Setting up application\n");

            Console.WriteLine("Mapping controllers\n");
            RouterKernel.UseRouting();

            Console.WriteLine("Setting up hosting server\n");
            IPAddress localAddr = IPAddress.Parse(host);
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("Waiting for requests\n");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(ProcessClient));
                thread.Start(client);
            }
        }

        static void ProcessClient(object parameter)
        {
            TcpClient client = (TcpClient)parameter;

            NetworkStream stream = client.GetStream();
            int count;
            Byte[] bytes = new Byte[1024];

            while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string data = Encoding.ASCII.GetString(bytes, 0, count);
                string response = "";

                try
                {
                    HttpRequest request = new(data);

                    Console.WriteLine($"Received request: {request.Method} at url {request.Url} from User-agent {request.Headers.GetValueOrDefault("User-Agent", "Undefined")}");
                    response += RouterKernel.MapController(request);
                }
                catch (HttpException ex)
                {
                    Console.WriteLine(ex.ToString());
                    response = new JsonResponse(ex).ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    response = new JsonResponse(500, new {message = "The server ran into a problem"}).ToString();
                }

                Byte[] msg = Encoding.ASCII.GetBytes(response);
                stream.Write(msg, 0, msg.Length);
            }

            client.Close();
        }

        public static void RegisterDependency<Parent, Child>()
            where Parent : class
            where Child : Parent, new()
        {
            DIManager.RegisterDependency<Parent, Child>();
        }
    }
}
