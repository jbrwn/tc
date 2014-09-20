using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace TileCook.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // TMS routes
            config.Routes.MapHttpRoute(
                name: "TMSTile",
                routeTemplate: "TMS/{Version}/{TileMap}/{Z}/{X}/{Y}.{Format}",
                defaults: new { Controller = "TMS", action = "Tile"}
            );

            config.Routes.MapHttpRoute(
                name: "TMSTileMap",
                routeTemplate: "TMS/{Version}/{TileMap}",
                defaults: new { Controller = "TMS", action="TileMap"}
            );

            config.Routes.MapHttpRoute(
                name: "TMSService",
                routeTemplate: "TMS/{Version}",
                defaults: new { Controller = "TMS", action="Service"}
            );

            config.Routes.MapHttpRoute(
                name: "TMSRoot",
                routeTemplate: "TMS",
                defaults: new { Controller = "TMS", action = "Root"}
            );

            // WMTS routes
            config.Routes.MapHttpRoute(
                name: "WMTSGetTile",
                routeTemplate: "WMTS/{Version}/{Layer}/{Style}/{TileMatrixSet}/{TileMatrix}/{TileRow}/{TileCol}.{Format}",
                defaults: new { Controller = "WMTS", action = "GetTile" }
            );

            config.Routes.MapHttpRoute(
                name: "WMTSGetCapabilities",
                routeTemplate: "WMTS/{Version}/WMTSCapabilities.xml",
                defaults: new { Controller = "WMTS", action = "GetCapabilities" }
            );

            // TileJSON routes
            config.Routes.MapHttpRoute(
                name: "TileJSON",
                routeTemplate: "TileJSON/{layer}.json",
                defaults: new { Controller = "TileJSON", action = "GetTileJSON" }
            );

            // Landing page
            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "",
                defaults: new { controller = "Default" }
            );

        }
    }
}
