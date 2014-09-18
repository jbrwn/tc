using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using TileCook.API;
using TileCook.Models;
using System.Configuration;

[assembly: OwinStartup(typeof(TileCook.API.SelfHost.Startup))]

namespace TileCook.API.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);

            //Mapnik config
            MapnikProvider.RegisterDatasources("mapnik/input");
            MapnikProvider.RegisterFonts("mapnik/fonts");

            //Bootstrap TileCook config file repository     
            string layerDir = ConfigurationManager.AppSettings["ConfigDirectory"] != null ?
                ConfigurationManager.AppSettings["ConfigDirectory"] : "Config";
            IPathResolver pathResolver = new PathResolver(layerDir);
            TileCookConfig.LoadConfigs(new LayerRepository(), layerDir);
        }
    }
}
