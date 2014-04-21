using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TileCook.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //TMS routes
            config.Routes.MapHttpRoute(
                name: "TMSTile",
                routeTemplate: "Services/TMS/{Version}/{TileMap}/{Z}/{X}/{Y}.{Format}",
                defaults: new { Controller = "TMS" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "TMS",
            //    routeTemplate: "Services/TMS/{Version}/{TileMap}",
            //    defaults: new { Controller = "TMS" }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "TMS",
            //    routeTemplate: "Services/TMS/{Version}",
            //    defaults: new { Controller = "TMS" }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "TMS",
            //    routeTemplate: "Services/TMS",
            //    defaults: new { Controller = "TMS" }
            //);

            //WMTS route
            config.Routes.MapHttpRoute(
                name: "WMTSGetTile",
                routeTemplate: "Services/WMTS/{Version}/{Layer}/{Style}/{TileMatrixSet}/{TileMatrix}/{TileRow}/{TileCol}.{Format}",
                defaults: new { Controller = "WMTS", action = "GetTile" }
            );

            config.Routes.MapHttpRoute(
                name: "WMTSGetCapabilities",
                routeTemplate: "Services/WMTS/{Version}/WMTSCapabilities.xml",
                defaults: new { Controller = "WMTS", action = "GetCapabilities" }
            );
        }
    }
}
