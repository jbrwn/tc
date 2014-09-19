using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Net.Http.Formatting;
using TileCook;
using TileCook.Models;
using TileCook.API.Models;
using TileCook.API.Filters;

namespace TileCook.API.Controllers
{
    [TMSExceptionFilter]
    public class TMSController : ApiController
    {
        // TO DO: inject repository through constructor
        private ILayerRepository _repository;

        public TMSController()
        {
            this._repository = new LayerRepository();
        }

        [HttpGet]
        [ActionName("Tile")]
        public HttpResponseMessage Tile(string Version, string TileMap, int? Z, int? X, int? Y, string Format)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested service VERSION {0} is not supported by this server", Version)
                );

            // Validate layer
            Layer layer = this._repository.Get(TileMap);
            if (layer == null)
                throw new InvalidRequestParameterException(
                    string.Format("The requested LAYER {0} does not exist on this server", TileMap)
                );

            // Validate tile coordinate paramaters
            if (Z == null)
                throw new InvalidRequestParameterException("Z paramater is not a valid integer");
            if (X == null)
                throw new InvalidRequestParameterException("X paramater is not a valid integer");
            if (Y == null)
                throw new InvalidRequestParameterException("Y paramater is not a valid integer");

            // Get image
            byte[] img = layer.GetTile(Z.Value, X.Value, Y.Value, Format);

            // Start response
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ByteArrayContent(img);
            
            // Set content type
            string mimeMapping = ContentType.GetContentType(Format);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeMapping);

            // Set encoding 
            response.Content.Headers.Add("Content-Encoding", ContentEncoding.GetContentEncoding(img));

            // Set browser cache control
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(layer.BrowserCache);

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

            return Request.CreateResponse(HttpStatusCode.OK, services, new XmlMediaTypeFormatter() { UseXmlSerializer = true });
        }

        [HttpGet]
        [ActionName("Service")]
        public HttpResponseMessage Service(string Version)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested service VERSION {0} is not supported by this server", Version)
                );
            
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
            foreach (Layer layer in this._repository.GetAll())
            {
                TileMapMetadata tileMapMetadata = new TileMapMetadata();
                tileMapMetadata.title = layer.Title;
                tileMapMetadata.srs = layer.Gridset.SRS;
                if (layer.Gridset.Name.Equals("GoogleMapsCompatible", StringComparison.OrdinalIgnoreCase))
                {
                    tileMapMetadata.profile = "global-mercator";
                }
                else
                {
                    tileMapMetadata.profile = "none";
                }
                tileMapMetadata.href = Url.Link("TMSTileMap", new { Version = "1.0.0", TileMap = layer.Name });
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
                throw new InvalidRequestParameterException(
                    string.Format("The requested service VERSION {0} is not supported by this server", Version)
                );

            // Validate layer
            Layer layer = this._repository.Get(TileMap);
            if (layer == null)
                throw new InvalidRequestParameterException(
                    string.Format("The requested LAYER {0} does not exist on this server", TileMap)
                );

            TileMap tileMap = new TileMap();
            tileMap.version = "1.0.0";
            tileMap.tilemapservice = Url.Link("TMSService", new { Version = "1.0.0" });
            tileMap.Title = layer.Title;
            tileMap.SRS = layer.Gridset.SRS;
            tileMap.BoudningBox = new BoundingBox();
            tileMap.BoudningBox.minx = layer.Bounds.Minx.ToString();
            tileMap.BoudningBox.miny = layer.Bounds.Miny.ToString();
            tileMap.BoudningBox.maxx = layer.Bounds.Maxx.ToString();
            tileMap.BoudningBox.maxy = layer.Bounds.Maxy.ToString();
            tileMap.Origin = new Origin();
            tileMap.Origin.x = layer.Bounds.Minx.ToString();
            tileMap.Origin.y = layer.Bounds.Miny.ToString();
            tileMap.TileFormat = new TileFormat();
            tileMap.TileFormat.width = layer.Gridset.TileHeight.ToString();
            tileMap.TileFormat.height = layer.Gridset.TileWidth.ToString();
            tileMap.TileFormat.mimetype = ContentType.GetContentType(layer.Formats[0]);
            tileMap.TileFormat.extension = layer.Formats[0];
            tileMap.TileSets = new TileSets();
            for(int i=0;i<layer.Gridset.Resolutions.Count;i++)
            {
                TileSet tileSet = new TileSet();
                tileSet.href = Url.Link("TMSTileMap", new { Version = "1.0.0", TileMap = layer.Name }) + "/" + i.ToString() ;
                tileSet.unitsperpixel = layer.Gridset.Resolutions[i].ToString();
                tileSet.order = i.ToString();
                tileMap.TileSets.Add(tileSet);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tileMap, Configuration.Formatters.XmlFormatter);
        }

    }
}