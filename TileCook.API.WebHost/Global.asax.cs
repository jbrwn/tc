//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Security;
//using System.Web.SessionState;

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

            // Get config file directory            
            string layerDir = WebConfigurationManager.AppSettings["ConfigDirectory"] != null ?
                WebConfigurationManager.AppSettings["ConfigDirectory"] : Server.MapPath("~/App_Data/Config");
            IPathResolver pathResolver = new PathResolver(layerDir);

            // Create configuration object map
            LayerMap LayerMap = new LayerMap(
                new GridSetMap(
                    new EnvelopeMap()
                ),
                new CacheMap(pathResolver),
                new ProviderMap(
                    new VectorTileLayerMap(
                        new GridSetMap(
                            new EnvelopeMap()
                        ),
                        new CacheMap(pathResolver),
                        new VectorTileProviderMap(pathResolver),
                        new EnvelopeMap()
                    ),
                    pathResolver
                ),
                new EnvelopeMap()
            );

            // Get layer repository
            ILayerRepository repo = new LayerRepository();

            // Deserialize config files
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                foreach (string file in Directory.EnumerateFiles(layerDir, "*.json", SearchOption.TopDirectoryOnly))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        using (JsonTextReader reader = new JsonTextReader(sr))
                        {
                            LayerConfig LayerConfig = (LayerConfig)serializer.Deserialize(reader, typeof(LayerConfig));
                            Layer l = LayerMap.Map(LayerConfig);
                            repo.Put(l);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceWarning(ex)
            }
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