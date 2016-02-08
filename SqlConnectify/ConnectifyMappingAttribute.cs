using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlConnectify
{
    public class ConnectifyMappingAttribute : Attribute
    {
        public string Column { get; set; }
    }
}
