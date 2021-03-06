﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TileCook.API.Models;
using TileCook.API.Formatting;
using TileCook.API.Filters;
using System.Xml.Serialization;
using TileCook.Models;

namespace TileCook.API.Controllers
{
    [WMTSExceptionFilter]
    public class WMTSController : ApiController
    {
        private ILayerRepository _repository;

        public WMTSController()
        {
            this._repository = new LayerRepository();
        }

        [HttpGet]
        [ActionName("GetTile")]
        public HttpResponseMessage GetTile(string Version, string Layer, string Style, string TileMatrixSet, int? TileMatrix, int? TileRow, int? TileCol, string Format)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested service VERSION {0} is not supported by this server", Version),
                    "Version"
                );

            // Validate layer
            Layer layer = this._repository.Get(Layer);
            if (layer == null)
                throw new InvalidRequestParameterException(
                    string.Format("The requested LAYER {0} does not exist on this server", Layer),
                    "Layer"
                );

            // Validate style
            if (!Style.Equals("default", StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested STYLE {0} is Invalid for LAYER {1}", Style, Layer),
                    "Style"
                );
            
            // Validate TileMatrixSet
            if (!TileMatrixSet.Equals(layer.GridSet.Name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested TILEMATRIXSET {0} is Invalid for LAYER {1}", TileMatrixSet, Layer),
                    "TileMatrixSet"
                );
            
            // Validate tile coordinate parameters
            if (TileMatrix == null)
                throw new InvalidRequestParameterException("TILEMATRIX paramater is not a valid integer","TileMatrix");
            if (TileCol == null)
                throw new InvalidRequestParameterException("TILECOL paramater is not a valid integer", "TileCol");
            if (TileRow == null)
                throw new InvalidRequestParameterException("TILEROW paramater is not a valid integer", "TileRow");


            // Get image
            byte[] img = layer.GetTile(new Coord(TileMatrix.Value, TileCol.Value, TileRow.Value,true), Format);

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
            //response.Headers.CacheControl = new CacheControlHeaderValue();
            //response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(layer.BrowserCache);

            return response;

        }

        [HttpGet]
        [ActionName("GetCapabilities")]
        public HttpResponseMessage GetCapabilities(string Version)
        {
            // Validate version
            if (!Version.Equals("1.0.0", StringComparison.OrdinalIgnoreCase))
                throw new InvalidRequestParameterException(
                    string.Format("The requested service VERSION {0} is not supported by this server",Version),
                    "Version"
                );


            IList<Layer> layers = (IList<Layer>)this._repository.GetAll();
            Dictionary<string, IGridSet> uniqueGridSets = new Dictionary<string, IGridSet>();
            foreach (Layer l in layers)
            {
                if (!uniqueGridSets.ContainsKey(l.GridSet.Name))
                {
                    uniqueGridSets[l.GridSet.Name] = l.GridSet;
                }
            }
            List<IGridSet> GridSets = new List<IGridSet>(uniqueGridSets.Values);
            
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
                LayerType.BoundingBox[0].crs = layers[i].GridSet.SRS;
                LayerType.BoundingBox[0].LowerCorner = layers[i].GridSet.Envelope.Minx.ToString() + " " + layers[i].GridSet.Envelope.Miny.ToString();
                LayerType.BoundingBox[0].UpperCorner = layers[i].GridSet.Envelope.Maxx.ToString() + " " + layers[i].GridSet.Envelope.Maxy.ToString();
                LayerType.Style = new Style[] {new Style() };
                LayerType.Style[0].isDefault = true;
                LayerType.Style[0].Identifier = new CodeType();
                LayerType.Style[0].Identifier.Value = "default";
                LayerType.Format = new string[layers[i].Provider.GetFormats().Count];
                for (int j = 0; j < layers[i].Provider.GetFormats().Count;j++ )
                {
                    LayerType.Format[j] = ContentType.GetContentType(layers[i].Provider.GetFormats()[j]);
                }
                LayerType.TileMatrixSetLink = new TileMatrixSetLink[] { new TileMatrixSetLink() };
                LayerType.TileMatrixSetLink[0].TileMatrixSet = layers[i].GridSet.Name;
                if (!layers[i].GridSet.Envelope.Equals(layers[i].GridSet.Envelope))
                {
                    int zLevels = layers[i].GridSet.Resolutions.Count;
                    LayerType.TileMatrixSetLink[0].TileMatrixSetLimits = new TileMatrixLimits[zLevels];
                    for (int j = 0; j < zLevels; j++)
                    {
                        int z = j;//layers[i].MinZoom + j;
                        Coord lowCoord = layers[i].GridSet.PointToCoord(new Point(layers[i].GridSet.Envelope.Minx, layers[i].GridSet.Envelope.Miny), z);
                        Coord highCoord = layers[i].GridSet.PointToCoord(new Point(layers[i].GridSet.Envelope.Maxx, layers[i].GridSet.Envelope.Maxy), z);
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j] = new TileMatrixLimits();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].TileMatrix = z.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MinTileRow = lowCoord.X.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MaxTileRow = highCoord.X.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MinTileCol = lowCoord.Y.ToString();
                        LayerType.TileMatrixSetLink[0].TileMatrixSetLimits[j].MaxTileCol = highCoord.Y.ToString();
                    }
                }
                LayerType.ResourceURL = new URLTemplateType[layers[i].Provider.GetFormats().Count];
                for (int j = 0; j < layers[i].Provider.GetFormats().Count; j++)
                {
                    LayerType.ResourceURL[j] = new URLTemplateType();
                    LayerType.ResourceURL[j].format = ContentType.GetContentType(layers[i].Provider.GetFormats()[j]);
                    LayerType.ResourceURL[j].resourceType = URLTemplateTypeResourceType.tile;
                    LayerType.ResourceURL[j].template = WebUtility.UrlDecode(Url.Link("WMTSGetTile", new { 
                        Version = "1.0.0", 
                        Layer = layers[i].Name, 
                        Style = "default", 
                        TileMatrixSet = layers[i].GridSet.Name,
                        TileMatrix = "{TileMatrix}",
                        TileRow = "{TileRow}",
                        TileCol = "{TileCol}",
                        Format = layers[i].Provider.GetFormats()[j]
                    }));
                }
            }
            capabilities.Contents.TileMatrixSet = new TileMatrixSet[GridSets.Count];
            for (int i = 0; i < GridSets.Count; i++)
            {
                capabilities.Contents.TileMatrixSet[i] = new TileMatrixSet();
                capabilities.Contents.TileMatrixSet[i].Identifier = new CodeType();
                capabilities.Contents.TileMatrixSet[i].Identifier.Value = GridSets[i].Name;
                capabilities.Contents.TileMatrixSet[i].SupportedCRS = GridSets[i].SRS;
                if (GridSets[i].Name.Equals("GoogleMapsCompatible", StringComparison.OrdinalIgnoreCase) ||
                    GridSets[i].Name.Equals("GlobalCRS84Scale", StringComparison.OrdinalIgnoreCase) ||
                    GridSets[i].Name.Equals("GlobalCRS84Pixel", StringComparison.OrdinalIgnoreCase) ||
                    GridSets[i].Name.Equals("GGoogleCRS84Quad", StringComparison.OrdinalIgnoreCase)
                )
                {
                    capabilities.Contents.TileMatrixSet[i].WellKnownScaleSet = GridSets[i].Name;
                }
                capabilities.Contents.TileMatrixSet[i].TileMatrix = new TileMatrix[GridSets[i].Resolutions.Count];
                for (int j = 0; j < GridSets[i].Resolutions.Count; j++)
                {
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j] = new TileMatrix();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].ScaleDenominator = GridSets[i].Resolutions[j] * GridSets[i].PixelSize;
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].Identifier = new CodeType();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].Identifier.Value = GridSets[i].Resolutions[j].ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TopLeftCorner = GridSets[i].Envelope.Minx.ToString() + " " + GridSets[i].Envelope.Maxy.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TileWidth = GridSets[i].TileWidth.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].TileHeight = GridSets[i].TileHeight.ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].MatrixWidth = GridSets[i].GridWidth(j).ToString();
                    capabilities.Contents.TileMatrixSet[i].TileMatrix[j].MatrixHeight = GridSets[i].GridHeight(j).ToString();
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
           
    }
}