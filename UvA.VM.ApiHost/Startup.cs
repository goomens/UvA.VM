using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace UvA.VM.ApiHost
{
    class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Web API configuration and services

            // Web API routes
            HttpConfiguration config = new HttpConfiguration();
            //config.MapHttpAttributeRoutes();
            config.EnableCors();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);
        }
    }
}
