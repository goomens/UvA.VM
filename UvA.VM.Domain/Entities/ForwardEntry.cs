using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UvA.VM.Domain.Entities
{
    public class ForwardEntry
    {
        public int ListenPort { get; set; }
        public int ConnectPort { get; set; }
        public string ConnectAddress { get; set; }
    }
}
