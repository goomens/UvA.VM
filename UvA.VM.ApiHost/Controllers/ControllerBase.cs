using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UvA.VM.Domain;

namespace UvA.VM.ApiHost.Controllers
{
    public abstract class ControllerBase : ApiController
    {
        protected Connector Connector;

        public ControllerBase()
        {
            Connector = new Connector(ConfigurationManager.AppSettings["TargetMachine"]);
        }
    }
}
