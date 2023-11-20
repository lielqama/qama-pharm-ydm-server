using PharmYdm.Providers.CommunicationProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PharmYdm
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CommunicationFactory.MicropayConfig = new MicropayConfig()
            {
                UserName = ConfigurationManager.AppSettings["micropay_username"],
                Number = ConfigurationManager.AppSettings["micropay_number"],
                UserID = ConfigurationManager.AppSettings["micropay_userID"]
            };
        }
    }
}
