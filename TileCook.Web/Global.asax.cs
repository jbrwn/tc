using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Configuration;
using TileCook;

namespace TileCook.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Mapnik config
            MapnikProvider.RegisterDatasources(WebConfigurationManager.AppSettings["mapnikInput"]);
            MapnikProvider.RegisterFonts(WebConfigurationManager.AppSettings["mapnikFonts"]);

            //TileCook config
            string configDir = Server.MapPath("~/App_Data/Config");
            WellKnownScaleSet.RegisterDirectory(Path.Combine(configDir,"WellKnownScaleSets"));
            LayerCache.ConfigDirectory = configDir;

            foreach (string file in Directory.EnumerateFiles(LayerCache.ConfigDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                try
                {

                    LayerCache.RegisterFile(file);
                }
                catch (Exception e)
                {
                    //log failed config load
                }
            }

        }
    }
}