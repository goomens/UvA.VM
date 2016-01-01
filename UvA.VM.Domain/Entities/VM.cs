using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UvA.VM.Domain.Entities
{
    public class VMObject
    {
        public string Name { get; set; }
        public int CPUUsage { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string Heartbeat { get; set; }
        public TimeSpan UpTime { get; set; }
        public string MacAddress { get; set; }
        public long MemoryAssigned { get; set; }
    }

    public class VMInitObject
    {
        public string Name { get; set; }
        public Int64 Memory { get; set; }
        public string ImageName { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
    }
}
