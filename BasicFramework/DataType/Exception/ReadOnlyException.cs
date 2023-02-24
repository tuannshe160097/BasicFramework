using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.DataType.Exception
{
    public class ReadOnlyException : System.Exception
    {
        public ReadOnlyException() : base("Cannot modify readonly object") { }
    }
}
