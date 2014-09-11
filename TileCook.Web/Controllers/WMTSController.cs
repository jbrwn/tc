using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TileCook.Web.WMTSService;
using TileCook.Web.Formatting;
using System.Xml.Serialization;
using TileCook.Web.Models;

namespace TileCook.Web.Controllers
{
    public class WMTSController : ApiController
    {
        private ILayerRepository _repository;

        public WMTSController()
        {
            this._repository = new LayerRepository();
        }

        [HttpGet]
        [ActionName("GetTile")]
        public HttpResponseMessage GetTile(string Version, string Layer, string Style, string TileMatrixSet, int TileMatrix, int TileRow, int TileCol, string Format)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "InvalidParameterValue",
                    string.Format("The requested service VERSION {0} is not supported by this server",Version),
                    "Version"
                );
            }

            // Validate layer
            Layer layer = this._repository.Get(Layer);
            if (layer == null)
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "InvalidParameterValue",
                    string.Format("The requested LAYER {0} does not exist on this server", Layer),
                    "Layer"
                );
            }

            // Validate style
            if (!Style.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "InvalidParameterValue",
                    string.Format("The requested STYLE {0} is Invalid for LAYER {1}", Style, Layer),
                    "Style"
                );
            }

            if (!TileMatrixSet.Equals(layer.Gridset.Name, StringComparison.OrdinalIgnoreCase))
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "InvalidParameterValue",
                    string.Format("The requested TILEMATRIXSET {0} is Invalid for LAYER {1}", TileMatrixSet, Layer),
                    "TileMatrixSet"
                );
            }

            // Get image
            byte[] img;
            try
            {
                TileRow = layer.FlipY(TileMatrix, TileRow);
                img = layer.GetTile(TileMatrix, TileCol, TileRow, Format);
            }
            catch (TileOutOfRangeException e)
            {
                string locator = e.Message.Split(' ')[0];
                switch (locator)
                {
                    case "Zoom":
                        locator = "TileMatrix";
                        break;
                    case "Column":
                        locator = "TileCol";
                        break;
                    case "Row":
                        locator = "TileRow";
                        break;
                }
                return GenerateOWSException(
                    HttpStatusCode.BadRequest,
                    "TileOutOfRange",
                    e.Message,
                    locator
                );
            }
            catch (InvalidTileFormatException e)
            {
                return GenerateOWSException(
                    HttpStatusCode.BadRequest,
                    "InvalidParameterValue",
                    e.Message,
                    "Format"
                );
            }

            // Check for null image
            if (img == null)
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "FileNotFound",
                    string.Format("You requested a map tile /{0}/{1}/{2} that does not exist.", TileMatrix, TileCol, TileRow),
                    ""
                );
            }

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
        [ActionName("GetCapabilities")]
        public HttpResponseMessage GetCapabilities(string Version)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateOWSException(
                    HttpStatusCode.NotFound,
                    "InvalidParameterValue",
                    "The requested service version is not supported by this server",
                    "Version"
                );
            }

            IList<Layer> layers = (IList<Layer>)this._repository.GetAll();
            Dictionary<string, IGridSet> uniqueGridSets = new Dictionary<string, IGridSet>();
            foreach (Layer l in layers)
            {
                if (!uniqueGridSets.ContainsKey(l.Gridset.Name))
                {
                    uniqueGridSets[l.Gridset.Name] = l.Gridset;
                }
            }
            List<IGridSet> gridSets = new List<IGridSet>(uniqueGridSets.Values);
            
            // Start capabilities 
            Capabilities capabilities = new Capabilities();
            capabilities.version = "1.0.0";
            
            // ServiceMetadataURL is only set for REST
            capabilities.ServiceMetadataURL = new OnlineResourceType[]{ new OnlineResourceType() };
            capabilities.ServiceMetadataURL[0].href = Url.Link("WMTSGetCapabilities", new { Version = "1.0.0" });

            // ServiceIdentification
            capabilities.ServiceIdentification = new ServiceIdentification();
            capabilities.ServiceIdentification.Title = new LanguageStringType[] {new LanguageStringType()};
            capabilities.ServiceIdentification.Title[0].Value = "Web Map Tile Service";
            capabilities.ServiceIdentification.ServiceType = new CodeType();
            capabilities.ServiceIdentification.ServiceType.Value = "OGC WMTS";
            capabilities.ServiceIdentification.ServiceTypeVersion = new string[] { "1.0.0" }; 

            // ServiceProvider
            capabilities.ServiceProvider = null;

            // Operations metadata only needed for KVP or SOAP
            capabilities.OperationsMetadata = null;

            // Contents
            capabilities.Contents = new ContentsType();
            capabilities.Contents.DatasetDescriptionSummary = new LayerType[layers.Count];
            for(int i=0; i<layers.Count;i++)
            {
                capabilities.Contents.DatasetDescriptionSummary[i] = new LayerType();
                LayerType LayerType = (LayerType)capabilities.Contents.DatasetDescriptionSummary[i];
                LayerType.Identifier = new CodeType();
                LayerType.Identifier.Value = layers[i].Name;
                LayerType.Title = new LanguageStringType[] { new LanguageStringType() };
                LayerType.Title[0].Value = layers[i].Title;
                LayerType.BoundingBox = new BoundingBoxType[] { new BoundingBoxType() };
                LayerType.BoundingBox[0].crs = layers[i].Gridset.SRS;
                LayerType.BoundingBox[0].LowerCorner = layers[i].Bounds.Minx.ToString() + " " + layers[i].Bounds.Miny.ToString();
                LayerType.BoundingBox[0].UpperCorner = layers[i].Bounds.Maxx.ToString() + " " + layers[i].Bounds.Maxy.ToString();
                LayerType.Style = new Style[] {new Style() };
                LayerType.Style[0].isDefault = true;
                LayerType.Style[0].Identifier = new CodeType();
                LayerType.Style[0].Identifier.Value = "default";
                LayerType.Format = new string[layers[i].Formats.Count];
                for (int j = 0; j < layers[i].Formats.Count;j++ )
                {
                    LayerType.Format[j] = ContentType.GetContentType(layers[i].Formats[j]);
                }
                LayerType.TileMatrixSetLink = new TileMatrixSetLink[] { new TileMatrixSetLink() };
                LayerType.TileMatrixSetLink[0].TileMatrixSet = layers[i].Gridset.Name;
                if (!layers[i].Bounds.Equals(layers[i].Gridset.Envelope))
                {
                    int zLevels = layers[i].MaxZoom - layers[i].MinZoom;
                    LayerType.TileMatrixSetLink[0].TileMatrixSetLimits = new TileMatrixLimits[zLevels];
                    for (int j = 0; j < zLevels; j++)
                    {
                        int z = layers[i].MinZoom + j;
                        Grid g = layers[i].Gridset.Grids[z];
                        Coord lowCoord = layers[i].Gridset.PointToCoord(new Point(layers[i].Bounds.Minx, layers[i].Bounds.Miny), z);
                        Coord highCoord = layers[i].Gridset.PointToCoord(new Point(layers[i].Bounds.Maxx, layers[i].Bounds.Maxy), z);
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j] = new TileMatrixLimits();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].TileMatrix = (layers[i].MinZoom + j).ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MinTileRow = lowCoord.X.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MaxTileRow = highCoord.X.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MinTileCol = lowCoord.Y.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MaxTileCol = highCoord.Y.ToString();
                    }
                }
                LayerType.ResourceURL = new URLTemplateType[layers[i].Formats.Count];
                for (int j = 0; j < layers[i].Formats.Count; j++)
                {
                    LayerType.ResourceURL[j] = new URLTemplateType();
                    LayerType.ResourceURL[j].format = ContentType.GetContentType(layers[i].Formats[j]);
                    LayerType.ResourceURL[j].resourceType = URLTemplateTypeResourceType.tile;
                    LayerType.ResourceURL[j].template = HttpUtility.UrlDecode(Url.Link("WMTSGetTile", new { 
                        Version = "1.0.0", 
                        Layer = layers[i].Name, 
                        Style = "default", 
                        TileMatrixSet = layers[i].Gridset.Name,
                        TileMatrix = "{TileMatrix}",
                        TileRow = "{TileRow}",
                        TileCol = "{TileCol}",
                        Format = layers[i].Formats[j]
                    }));
                }
            }
            capabilities.Contents.TileMatrixSet = new TileMatrixSet[gridSets.Count];
            for (int i = 0; i < gridSets.Count; i++)
            {
                capabilities.Contents.TileMatrixSet[i] = new TileMatrixSet();
                capabilities.Contents.TileMatrixSet[i].Identifier = new CodeType();
                capabilities.Contents.TileMatrixSet[i].Identifier.Value = gridSets[i].Name;
                capabilities.Contents.TileMatrixSet[i].SupportedCRS = gridSets[i].SRS;
                if (gridSets[i].Name.Equals("GoogleMapsCompatible", StringComparison.OrdinalIgnoreCase) ||
                    gridSets[i].Name.Equals("GlobalCRS84Scale", StringComparison.OrdinalIgnoreCase) ||
                    gridSets[i].Name.Equals("GlobalCRS84Pixel", StringComparison.OrdinalIgnoreCase) ||
                    gridSets[i].Name.Equals("GGoogleCRS84Quad", StringComparison.OrdinalIgnoreCase)
                )
                {
                    capabilities.Contents.TileMatrixSet[i].WellKnownScaleSet = gridSets[i].Name;
                }
                capabilities.Contents.TileMatrixSet[i].TileMatrix = new TileMatrix[gridSets[i].Grids.Count];
                for (int j = 0; j < gridSets[i].Grids.Count; j++)
                {
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j] = new TileMatrix();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].ScaleDenominator = gridSets[i].Grids[j].Scale;
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].Identifier = new CodeType();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].Identifier.Value = gridSets[i].Grids[j].Name;
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TopLeftCorner = gridSets[i].Envelope.Minx.ToString() + " " + gridSets[i].Envelope.Maxy.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TileWidth = gridSets[i].TileWidth.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TileHeight = gridSets[i].TileHeight.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].MatrixWidth = gridSets[i].GridWidth(j).ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].MatrixHeight = gridSets[i].GridHeight(j).ToString();
                }
            }

            // Serialize 
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.opengis.net/wmts/1.0");
            ns.Add("xlink", "http://www.w3.org/1999/xlink");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            ns.Add("ows", "http://www.opengis.net/ows/1.1");
            ns.Add("gml", "http://www.opengis.net/gml");

            return Request.CreateResponse(HttpStatusCode.OK, capabilities, new ExtendedXmlMediaTypeFormatter(ns));
        }

        private HttpResponseMessage GenerateOWSException(HttpStatusCode statusCode, string code, string message, string locator)
        {
            ExceptionReport ex = new ExceptionReport();
            ex.lang = "en";
            ex.version = "1.0.0";
            ex.Exception = new ExceptionType[] { new ExceptionType() };
            ex.Exception[0].ExceptionText = new string[] { message };
            ex.Exception[0].exceptionCode = code;
            ex.Exception[0].locator = locator;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("ows", "http://www.opengis.net/ows/1.1");
            return Request.CreateResponse(statusCode, ex, new ExtendedXmlMediaTypeFormatter(ns));           
        }
           
    }
}