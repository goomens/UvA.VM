using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using UvA.VM.Domain;
using UvA.VM.Domain.Entities;

namespace UvA.VM.ApiHost.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PortController : ApiController
    {
        NetConnector Connector;

        public PortController()
        {
            Connector = new NetConnector();
        }

        [HttpGet]
        public IEnumerable<ForwardEntry> GetEntries()
        {
            return Connector.GetEntries();
        }

        [HttpPost]
        public bool AddEntry(ForwardEntry entry)
        {
            Connector.AddForward(entry);
            return true;
        }

        [HttpDelete]
        public bool DeleteEntry(ForwardEntry entry)
        {
            Connector.RemoveForward(entry.ListenPort);
            return true;
        }
    }
}
