using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HydroServerTools
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces:new string[] { "HydroServerTools.Controllers" }
               
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                        name: "Activate",
                        url: "{controller}/{action}/{id}/{email}",
                        defaults: new { controller = "ServiceRegistrations", action = "Activation", id = UrlParameter.Optional, email = UrlParameter.Optional },
                        namespaces: new string[] { "HydroServerTools.Controllers" }

            );
        }
    }
}
