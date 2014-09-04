using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TileCook.Web.Models;
using TileCook.Web.TileJSONService;


namespace TileCook.Web.Controllers
{
    public class TileJSONController : ApiController
    {
        private ILayerRepository _repository;

        public TileJSONController()
        {
            this._repository = new LayerRepository();
        }
        
        [HttpGet]
        [ActionName("GetTileJSON")]
        public HttpResponseMessage GetTileJSON(string layer)
        {
            // Validate layer
            Layer tileLayer = this._repository.Get(layer);
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
            tilejson.name = tileLayer.Name;
            tilejson.descritpion = tileLayer.Title;
            tilejson.scheme = "tms";
            tilejson.minzoom = tileLayer.MinZoom;
            tilejson.maxzoom = tileLayer.MaxZoom;

            // Check for UTFGrid
            if (HasUTFGrid(tileLayer))
            {
                tilejson.grids = new List<string>() { HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = tileLayer.Name, Z = "{z}", X = "{x}", Y = "{y}", Format = "json" })) };
            }

            // Set tile endpoint
            tilejson.tiles = new List<string>() { HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = tileLayer.Name, Z = "{z}", X = "{x}", Y = "{y}", Format = format })) };

            // check for vector_layers info
            if (format == "pbf" && tileLayer.Provider is IVectorTileProvider)
            {
                IVectorTileProvider vectorProvider = (IVectorTileProvider)tileLayer.Provider;
                List<VectorLayerMetadata> layerMetadataList = vectorProvider.GetVectorTileMetadata();
                List<vector_layer> vlayers = new List<vector_layer>();
                foreach (VectorLayerMetadata layerMetadata in layerMetadataList)
                {
                    vector_layer vlayer = new vector_layer();
                    vlayer.id = layerMetadata.Name;
                    vlayer.descritpion = layerMetadata.Description;
                    vlayer.fields = layerMetadata.Fields;
                    vlayers.Add(vlayer);
                }
                tilejson.vector_layers = vlayers;
            }

            return Request.CreateResponse(HttpStatusCode.OK, tilejson, Configuration.Formatters.JsonFormatter);
        }

        private string GetDefaultFormat(Layer layer)
        {
            foreach (string format in layer.Formats)
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
            foreach (string format in layer.Formats)
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
