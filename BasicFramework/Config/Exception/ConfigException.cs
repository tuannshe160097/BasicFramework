using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Config.Exception
{
    internal class ConfigException : System.Exception
    {
        public ConfigException(string field) : base(field + "is incorrectly configured") { }
    }
}
