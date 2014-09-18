using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Configuration;
using System.Diagnostics;
using TileCook.Models;
using Newtonsoft.Json;

namespace TileCook.API.WebHost
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            //Mapnik config
            MapnikProvider.RegisterDatasources(Server.MapPath("~/bin/mapnik/input"));
            MapnikProvider.RegisterFonts(Server.MapPath("~/bin/mapnik/fonts"));

            //Bootstrap TileCook config file repository                
            string layerDir = WebConfigurationManager.AppSettings["ConfigDirectory"] != null ?
                WebConfigurationManager.AppSettings["ConfigDirectory"] : Server.MapPath("~/App_Data/Config");
            IPathResolver pathResolver = new PathResolver(layerDir);
            TileCookConfig.LoadConfigs(new LayerRepository(), layerDir);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}