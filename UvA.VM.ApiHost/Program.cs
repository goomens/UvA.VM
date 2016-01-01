using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UvA.VM.ApiHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string addr = ConfigurationManager.AppSettings["ListenAddress"];


            using (WebApp.Start<Startup>(addr))
            {
                Console.ReadLine();
            }
        }
    }
}
