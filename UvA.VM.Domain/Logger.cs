using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UvA.VM.Domain
{
    public class Logger
    {
        public static void Log(string caption, string message)
        {
            File.WriteAllText("Logs/" + DateTime.Now.ToString("yyyyMMddHHmmss") + " " + caption + ".log", message);
        }
    }
}
