using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicFramework.Network.Exception;

namespace BasicFramework.Network.Http
{
    public class HttpResponse
    {
        public int Status { get; set; }
        public string Protocol { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string? Body { get; set; }

        public HttpResponse(int status, string protocol, Dictionary<string, string> headers, string body)
        {
            Status = status;
            Protocol = protocol;
            Headers = headers;
            Body = body;
        }

        public HttpResponse(string body)
        {
            Status = 200;

            Protocol = "HTTP/1.1";

            Headers.Add("Date", DateTime.Now.ToString());
            Headers.Add("Content-Length", Encoding.ASCII.GetBytes(body).Length.ToString());
            Headers.Add("Content-Type", "text/html");
            Headers.Add("Connection", "close");

            Body = body;
        }

        public HttpResponse(int status)
        {
            Status = status;

            Protocol = "HTTP/1.1";

            Headers.Add("Date", DateTime.Now.ToString());
            Headers.Add("Content-Length", "0");
            Headers.Add("Connection", "close");
        }

        public HttpResponse(int status, string body)
        {
            Status = status;

            Protocol = "HTTP/1.1";

            Headers.Add("Date", DateTime.Now.ToString());
            Headers.Add("Content-Length", Encoding.ASCII.GetBytes(body).Length.ToString());
            Headers.Add("Content-Type", "text/html");
            Headers.Add("Connection", "close");

            Body = body;
        }

        public HttpResponse(HttpException ex)
        {
            Status = ex.HttpStatus;

            Protocol = "HTTP/1.1";

            Headers.Add("Date", DateTime.Now.ToString());
            Headers.Add("Connection", "close");

            if (ex.Response != null)
            {
                Headers.Add("Content-Type", "text/html");
                Headers.Add("Content-Length", Encoding.ASCII.GetBytes(ex.Response.ToString()).Length.ToString());
                Body = ex.Response.ToString();
            }
            else
            {
                Headers.Add("Content-Length", "0");
            }
        }

        public override string ToString()
        {
            string data = "";
            data += $"{Protocol} {Status} {HttpStatus.status[Status]}\n";

            foreach (var header in Headers)
            {
                data += $"{header.Key}: {header.Value}\n";
            }

            data += $"\n{Body}";

            return data;
        }
    }
}
