using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using UvA.VM.Domain;
using UvA.VM.Domain.Entities;

namespace UvA.VM.ApiHost.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VMController : ControllerBase
    {
        [HttpGet]
        public VMObject GetVM(string id)
        {
            return Connector.GetVM(id);
        }

        [HttpGet]
        public IEnumerable<VMObject> GetVMs()
        {
            return Connector.GetVMs();
        }

        [HttpPost]
        public bool CreateVM(VMInitObject vm)
        {
            Connector.CreateVM(vm);
            return true;
        }
    }
}
