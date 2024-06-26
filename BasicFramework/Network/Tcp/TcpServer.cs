﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BasicFramework.Network.Tcp
{
    using BasicFramework.Config;
    using BasicFramework.Kernel;
    using BasicFramework.Network.Exception;
    using BasicFramework.Network.Http;

    internal static class TcpServer
    {
        public static void StartServer()
        {
            Console.WriteLine("Setting up hosting server\n");
            IPAddress localAddr = IPAddress.Parse(Config.Host);
            TcpListener server = new TcpListener(localAddr, Config.Port);
            server.Start();

            Console.WriteLine($"Waiting for requests at {Config.Host}:{Config.Port}\n");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(ProcessClient));
                thread.Start(client);
            }
        }

        private static void ProcessClient(object parameter)
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
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    response = new JsonResponse(500, new { message = "The server ran into a problem" }).ToString();
                }

                Byte[] msg = Encoding.ASCII.GetBytes(response);
                stream.Write(msg, 0, msg.Length);
            }

            client.Close();
        }
    }
}
