using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.Config.Exception
{
    internal class ConfigNotFoundException : System.Exception
    {
        public ConfigNotFoundException() : base ("Configuration file not found") { }
    }
}
