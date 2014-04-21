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
    public class WMTSController : ApiController
    {
        [HttpGet]
        [ActionName("GetTile")]
        public HttpResponseMessage GetTile(string Version, string Layer, string Style, string TileMatrixSet, int TileMatrix, int TileRow, int TileCol, string Format)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Validate layer
            Layer layer = LayerCache.GetLayer(Layer);
            if (layer == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Validate style
            if (!Style.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (!TileMatrixSet.Equals(layer.gridset.name, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Flip Y
            TileCol = layer.gridset.gridHeight(TileMatrix) - TileCol - 1;

            // Get image
            byte[] img = layer.getTile(TileMatrix, TileRow, TileCol, Format);

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
        [ActionName("GetCapabilities")]
        public HttpResponseMessage GetCapabilities(string Version)
        {
            
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("");
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            return response;
        }

    }
}