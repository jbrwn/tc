using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TileCook;
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
                TileMapServiceError error = new TileMapServiceError();
                error.Message = string.Format("The requested service VERSION {0} is not supported by this server", Version);
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }

            // Validate layer
            Layer layer = LayerCache.GetLayer(TileMap);
            if (layer == null)
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = string.Format("The requested LAYER {0} does not exist on this server", TileMap);
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }
            
            // Get image
            byte[] img;
            try
            {
                img = layer.getTile(Z, X, Y, Format);
            }
            catch (TileOutOfRangeException e)
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = e.Message;
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }
            catch (InvalidTileFormatException e)
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = e.Message;
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }

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
            Services services = new Services();
            TileMapServiceMetadata metadata = new TileMapServiceMetadata();
            metadata.title = "Tile Map Service";
            metadata.version = "1.0.0";
            metadata.href = Url.Link("TMSService", new { Version = "1.0.0" });
            services.Add(metadata);

            return Request.CreateResponse(HttpStatusCode.OK, services, Configuration.Formatters.XmlFormatter);
        }

        [HttpGet]
        [ActionName("Service")]
        public HttpResponseMessage Service(string Version)
        {

            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = string.Format("The requested service VERSION {0} is not supported by this server", Version);
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }

            TileMapService tileMapService = new TileMapService();
            tileMapService.version = "1.0.0";
            tileMapService.services = Url.Link("TMSRoot", new { });
            //tileMapService.Title = null;
            //tileMapService.Abstract = null;
            //tileMapService.KeywordList = null;
            //tileMapService.ContactInformation = new ContactInformation();
            //tileMapService.ContactInformation.ContactPersonPrimary = new ContactPersonPrimary();
            //tileMapService.ContactInformation.ContactPersonPrimary.ContactPerson = null;
            //tileMapService.ContactInformation.ContactPersonPrimary.ContactOrganization = null;
            //tileMapService.ContactInformation.ContactPosition = null;
            //tileMapService.ContactInformation.ContactAddress = new ContactAddress();
            //tileMapService.ContactInformation.ContactAddress.AddressType = null;
            //tileMapService.ContactInformation.ContactAddress.Address = null;
            //tileMapService.ContactInformation.ContactAddress.City = null;
            //tileMapService.ContactInformation.ContactAddress.StateOrProvince = null;
            //tileMapService.ContactInformation.ContactAddress.PostCode = null;
            //tileMapService.ContactInformation.ContactAddress.Country = null;
            //tileMapService.ContactInformation.ContactVoiceTelephone = null;
            //tileMapService.ContactInformation.ContactFacsimileTelephone = null;
            //tileMapService.ContactInformation.ContactElectronicMailAddress = null;
            tileMapService.TileMaps = new List<TileMapMetadata>();
            foreach (KeyValuePair<string, Layer> item in LayerCache.GetLayers())
            {
                TileMapMetadata tileMapMetadata = new TileMapMetadata();
                tileMapMetadata.title = item.Value.Title;
                tileMapMetadata.srs = item.Value.gridset.srs;
                if (item.Value.gridset.name.Equals("GoogleMapsCompatible", StringComparison.OrdinalIgnoreCase))
                {
                    tileMapMetadata.profile = "global-mercator";
                }
                else
                {
                    tileMapMetadata.profile = "none";
                }
                tileMapMetadata.href = Url.Link("TMSTileMap", new { Version = "1.0.0", TileMap = item.Value.name });
                tileMapService.TileMaps.Add(tileMapMetadata);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tileMapService, Configuration.Formatters.XmlFormatter);
        }

        [HttpGet]
        [ActionName("TileMap")]
        public HttpResponseMessage TileMap(string Version, string TileMap)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = string.Format("The requested service VERSION {0} is not supported by this server", Version);
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }

            // Validate layer
            Layer layer = LayerCache.GetLayer(TileMap);
            if (layer == null)
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = string.Format("The requested LAYER {0} does not exist on this server", TileMap);
                return Request.CreateResponse(HttpStatusCode.NotFound, error, Configuration.Formatters.XmlFormatter);
            }

            TileMap tileMap = new TileMap();
            tileMap.version = "1.0.0";
            tileMap.tilemapservice = Url.Link("TMSService", new { Version = "1.0.0" });
            tileMap.Title = layer.Title;
            tileMap.SRS = layer.gridset.srs;
            tileMap.BoudningBox = new BoundingBox();
            tileMap.BoudningBox.minx = layer.bounds.minx.ToString();
            tileMap.BoudningBox.miny = layer.bounds.miny.ToString();
            tileMap.BoudningBox.maxx = layer.bounds.maxx.ToString();
            tileMap.BoudningBox.maxy = layer.bounds.maxy.ToString();
            tileMap.Origin = new Origin();
            tileMap.Origin.x = layer.bounds.minx.ToString();
            tileMap.Origin.y = layer.bounds.miny.ToString();
            tileMap.TileFormat = new TileFormat();
            tileMap.TileFormat.width = layer.gridset.tileHeight.ToString();
            tileMap.TileFormat.height = layer.gridset.tileWidth.ToString();
            tileMap.TileFormat.mimetype = System.Web.MimeMapping.GetMimeMapping("." + layer.formats[0]);
            tileMap.TileFormat.extension = layer.formats[0];
            tileMap.TileSets = new TileSets();
            for(int i=0;i<layer.gridset.grids.Count;i++)
            {
                TileSet tileSet = new TileSet();
                tileSet.href = Url.Link("TMSTileMap", new { Version = "1.0.0", TileMap = layer.name }) + "/" + i.ToString() ;
                tileSet.unitsperpixel = layer.gridset.resolution(i).ToString();
                tileSet.order = i.ToString();
                tileMap.TileSets.Add(tileSet);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tileMap, Configuration.Formatters.XmlFormatter);
        }

    }
}