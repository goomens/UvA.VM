using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace UvA.VM.ApiHost.Controllers 
{
    public class StartController : ControllerBase
    {
        [HttpPut]
        public bool StartVM(string id)
        {
            Connector.StartVM(id);
            return true;
        }
    }

    public class StopController : ControllerBase
    {
        [HttpPut]
        public bool StopVM(string id)
        {
            Connector.StopVM(id);
            return true;
        }
    }
}
