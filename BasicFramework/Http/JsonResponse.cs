using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Http
{
    public class JsonResponse : HttpResponse
    {
        public JsonResponse(object body) : base(JsonConvert.SerializeObject(body))
        {
            Headers["Content-Type"] = "application/json";
        }

        public JsonResponse(int status, object body) : base(status, JsonConvert.SerializeObject(body))
        {
            Headers["Content-Type"] = "application/json";
        }

        public JsonResponse(HttpException ex) : base(ex)
        {
            if (ex.Response != null)
            {
                Headers["Content-Type"] = "application/json";

                string response = JsonConvert.SerializeObject(ex.Response);

                Headers["Content-Length"] = Encoding.ASCII.GetBytes(response).Length.ToString();
                Body = response;
            }
        }
    }
}
