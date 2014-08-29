using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TileCook.Web.TileJSONService;


namespace TileCook.Web.Controllers
{
    public class TileJSONController : ApiController
    {
        [HttpGet]
        [ActionName("GetTileJSON")]
        public HttpResponseMessage GetTileJSON(string layer)
        {
            // Validate layer
            Layer tileLayer = LayerCache.GetLayer(layer);
            if (tileLayer == null)
            {
                string message = "Tileset does not exist";
                return Request.CreateResponse(HttpStatusCode.NotFound, message, Configuration.Formatters.JsonFormatter);
            }

            //TileJSON end point must have at least one tile format
            string format = GetDefaultFormat(tileLayer);
            if (format == null)
            {
                string message = "Tileset does not exist";
                return Request.CreateResponse(HttpStatusCode.NotFound, message, Configuration.Formatters.JsonFormatter);
            }

            // Create tilejson object
            TileJSON tilejson = new TileJSON();
            tilejson.tilejson = "2.1.0";
            tilejson.name = tileLayer.name;
            tilejson.descritpion = tileLayer.Title;
            tilejson.scheme = "tms";
            tilejson.minzoom = tileLayer.minZoom;
            tilejson.maxzoom = tileLayer.maxZoom;

            // Check for UTFGrid
            if (HasUTFGrid(tileLayer))
            {
                tilejson.grids = new List<string>() { HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = tileLayer.name, Z = "{z}", X = "{x}", Y = "{y}", Format = "json" })) };
            }

            // Set tile endpoint
            tilejson.tiles = new List<string>() { HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = tileLayer.name, Z = "{z}", X = "{x}", Y = "{y}", Format = format })) };

            // check for vector_layers info
            if (format == "pbf")
            {
                if (tileLayer.provider is IVectorTileProvider)
                {
                    IVectorTileProvider vectorProvider = (IVectorTileProvider)tileLayer.provider;
                    List<VectorLayerMetadata> vectorLayerMetadataList = vectorProvider.GetVectorTileMetadata();
                    List<vector_layer> vlayers = new List<vector_layer>();
                    foreach (VectorLayerMetadata vectorLayerMetadata in vectorLayerMetadataList)
                    {
                        vector_layer vlayer = new vector_layer();
                        vlayer.id = vectorLayerMetadata.Name;
                        vlayer.descritpion = vectorLayerMetadata.Description;
                        vlayer.fields = vectorLayerMetadata.Fields;
                        vlayers.Add(vlayer);
                    }
                    tilejson.vector_layers = vlayers;
                }
            }
  

            return Request.CreateResponse(HttpStatusCode.OK, tilejson, Configuration.Formatters.JsonFormatter);
        }

        private string GetDefaultFormat(Layer layer)
        {
            foreach (string format in layer.formats)
            {
                if (format.ToLower() != "json")
                {
                    return format.ToLower();
                }   
            }
            return null;
        }

        private bool HasUTFGrid(Layer layer)
        {
            foreach (string format in layer.formats)
            {
                if (format.ToLower() == "json")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
