using Newtonsoft.Json;
using System.Dynamic;

namespace BasicFramework.Http
{
    public class HttpRequest
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public string Protocol { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
        public string BodyRaw { get; set; }
        //This is an ExpandoObject
        public dynamic Body { get; set; }

        public HttpRequest(string data)
        {
            if (data == null)
            {
                throw new Exception("Empty data");
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

                    var body = new ExpandoObject() as IDictionary<string, object>;

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
                Body = JsonConvert.DeserializeObject<ExpandoObject>(body);
            }
        }
    }
}
