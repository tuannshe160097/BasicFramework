using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Config.Exception
{
    internal class ConfigNullException : System.Exception
    {
        public ConfigNullException(string field) : base(field + " cannot be empty") { }
    }
}
