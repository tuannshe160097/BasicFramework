using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Network.Exception
{
    internal class EmptyRequestException : System.Exception
    {
        public EmptyRequestException() : base("This request is empty") { }
    }
}
