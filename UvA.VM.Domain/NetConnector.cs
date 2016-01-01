using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UvA.VM.Domain.Entities;

namespace UvA.VM.Domain
{
    public class NetConnector
    {
        static readonly string LocalPrefix = "10.0";

        string GetIP()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(e => e.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                && !e.ToString().StartsWith(LocalPrefix)).ToString();
        }

        public void AddForward(ForwardEntry entry)
        {
            if (!entry.ConnectAddress.StartsWith(LocalPrefix))
                throw new InvalidOperationException("Should be local address");
            Console.WriteLine(Run("interface portproxy add v4tov4 listenaddress={0} listenport={1} connectaddress={2} connectport={3}",
                GetIP(), entry.ListenPort, entry.ConnectAddress, entry.ConnectPort));
        }

        public void RemoveForward(int port)
        {
            Console.WriteLine(Run("interface portproxy delete v4tov4 listenaddress={0} listenport={1}", GetIP(), port));
        }

        public ForwardEntry[] GetEntries()
        {
            var ip = GetIP();
            var lines = Run("interface portproxy show v4tov4").Split('\n');
            return lines.Where(t => t.StartsWith(ip)).Select(l =>
                {
                    var parts = l.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return new ForwardEntry()
                    {
                        ConnectAddress = parts[2],
                        ListenPort = int.Parse(parts[1]),
                        ConnectPort = int.Parse(parts[3])
                    };
                }).ToArray();
        }

        string Run(string cmd, params object[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo("netsh", string.Format(cmd, args));
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            var p = Process.Start(info);
            return p.StandardOutput.ReadToEnd();
        }
    }
}
