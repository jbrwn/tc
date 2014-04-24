using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TileCook.Web.TMSService;

namespace TileCook.Web.Controllers
{
    public class TMSController : ApiController
    {

        [HttpGet]
        [ActionName("Tile")]
        public HttpResponseMessage Tile(string Version, string TileMap, int Z, int X, int Y, string Format)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Validate layer
            Layer layer = LayerCache.GetLayer(TileMap);
            if (layer == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Get image
            byte[] img = layer.getTile(Z, X, Y, Format);

            // Start response
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ByteArrayContent(img);
            
            // Set content type
            string mimeMapping = System.Web.MimeMapping.GetMimeMapping("." + Format);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeMapping);

            // Set browser cache control
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(layer.browserCache);

            return response;

        }

        [HttpGet]
        [ActionName("Root")]
        public HttpResponseMessage Root()
        {
            string title = "Tile Map Service";
            string version = "1.0.0";
            string href = Request.RequestUri.ToString() + version;
            Services services = new Services();
            services.Add(new TileMapServiceMetadata(title, version, href));
            return Request.CreateResponse(HttpStatusCode.OK, services, Configuration.Formatters.XmlFormatter);
        }

        [HttpGet]
        [ActionName("Service")]
        public HttpResponseMessage Service(string Version)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("TileMap")]
        public HttpResponseMessage TileMap(string Version, string TileMap)
        {
            throw new NotImplementedException();
        }

    }
}