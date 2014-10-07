using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProxyServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("HealthTest", "Proxy/HealthTest", new { controller = "Proxy", action = "HealthTest" });

            routes.MapRoute("HttpProxy", "Proxy/{*path}", new { controller = "Proxy", action = "ProcessRequest" });
        }
    }
}