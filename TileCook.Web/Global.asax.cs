using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Configuration;
using TileCook.Web.Models;
using Newtonsoft.Json;


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
            MapnikProvider.RegisterDatasources(Server.MapPath("~/bin/mapnik/input"));
            MapnikProvider.RegisterFonts(Server.MapPath("~/bin/mapnik/fonts"));

            // Load layer repository
            ILayerRepository repo = new LayerRepository();
            LayerDTOMap layerDTOMap = new LayerDTOMap();
            
            string layerDir = Server.MapPath("~/App_Data/Config");
            JsonSerializer serializer = new JsonSerializer();
            foreach (string file in Directory.EnumerateFiles(layerDir, "*.json", SearchOption.TopDirectoryOnly))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        LayerDTO layerDTO = (LayerDTO)serializer.Deserialize(reader, typeof(LayerDTO));
                        Layer l = layerDTOMap.Map(layerDTO);
                        repo.Put(l);
                    }
                }
            }
        }
    }
}