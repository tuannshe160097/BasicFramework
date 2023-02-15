using BasicFramework.Network.Exception;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace BasicFramework.Network.Http
{
    public class HttpRequest
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public string Protocol { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
        public string BodyRaw { get; set; }

        //JObject
        public dynamic Body { get; set; }

        public HttpRequest(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new EmptyRequestException();
            }

            StringReader reader = new(data);

            //First line------------------------------------------------------------------------------
            string[] startLine = reader.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            Method = Enum.Parse<HttpMethod>(startLine[0], true);

            string[] urlWithParam = startLine[1].Split("?", 2);
            Url = urlWithParam[0];

            Protocol = startLine[2];

            //Headers---------------------------------------------------------------------------------
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                string[] headerLine = line.Split(":");
                Headers.Add(headerLine[0], headerLine[1]);
            }

            //Body------------------------------------------------------------------------------------
            if (Method == HttpMethod.GET)
            {
                if (urlWithParam.Length > 1)
                {
                    BodyRaw = urlWithParam[1];

                    var body = new Dictionary<string, object>();

                    string[] parameters = urlWithParam[1].Split("&");
                    foreach (string parameter in parameters)
                    {
                        string[] keyValue = parameter.Split("=", StringSplitOptions.RemoveEmptyEntries);
                        body.Add(keyValue[0], keyValue[1]);
                    }

                    Body = body;
                }
            }
            else
            {
                string body = reader.ReadToEnd();

                BodyRaw = body;

                //@TODO Add support for header Content-Type
                Body = JObject.Parse(body);
            }
        }
    }
}
