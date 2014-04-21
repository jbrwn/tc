using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace TileCook.Web.Controllers
{
    public class TMSController : ApiController
    {
        public HttpResponseMessage Get(string Version, string TileMap, int Z, int X, int Y, string Format)
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
    }
}