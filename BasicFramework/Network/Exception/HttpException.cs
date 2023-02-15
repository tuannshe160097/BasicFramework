using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Network.Exception
{
    public class HttpException : System.Exception
    {
        public int HttpStatus { get; set; }
        public object? Response { get; set; }

        public HttpException(int httpStatus)
        {
            HttpStatus = httpStatus;
        }

        public HttpException(int httpStatus, object response)
        {
            HttpStatus = httpStatus;
            Response = response;
        }

        public HttpException(int httpStatus, object response, string message) : base(message)
        {
            HttpStatus = httpStatus;
            Response = response;
        }
    }
}
