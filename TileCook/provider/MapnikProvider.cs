using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NETMapnik;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Ionic.Zlib;

namespace TileCook
{
    public class MapnikProvider : IEnvelopeProvider, IVectorTileProvider
    {
        private Map _map;
        private static readonly Object mapLock = new Object();
        private string _pngOptions;
        private string _jpegOptions;
        private int _gridLayerIndex;
        private int _gridResolution;
        private List<string> _gridFields;
        private string _compression;

        public MapnikProvider(string xmlConfig)
            : this(xmlConfig, 0, null, null, 0, 0, null, null) { }


        public MapnikProvider(string xmlConfig, int buffer, string pngOptions, string jpegOptions, int gridLayerIndex, int gridResolution, List<string> gridFields, string compression)
        {
            if (string.IsNullOrEmpty(xmlConfig))
            {
                throw new ArgumentNullException("MapnikProvider xmlConfig cannot be null or empty");
            }

            // set jpegOptions
            if (jpegOptions == null) 
            { 
                this._jpegOptions = "jpeg"; 
            }
            else
            {
                this._jpegOptions = jpegOptions;
            }

            // set pngOptions
            if (pngOptions == null) 
            {
                this._pngOptions = "png"; 
            }
            else
            {
                this._pngOptions = pngOptions;
            }

            // set gridLayerIndex
            this._gridLayerIndex = gridLayerIndex;

            // set gridResolution
            if (gridResolution == 0) 
            {
                this._gridResolution = 4; 
            }
            else
            {
                this._gridResolution = gridResolution;
            }

            // set gridFields
            if (gridFields == null)
            { 
                this._gridFields = new List<string>(); 
            }
            else
            {
                this._gridFields = gridFields;
            }

            // set compression
            if (compression == null) 
            { 
                this._compression = "gzip"; 
            }
            else
            {
                this._compression = compression;
            }

            // create internal mapnik object
            this._map = new Map();
            _map.LoadMap(xmlConfig);
            this._map.Buffer = buffer;
        }

        public string JpegOptions { get { return this._jpegOptions; } }
        public string PngOptions { get { return this._pngOptions; } }
        public int GridLayerIndex { get { return this._gridLayerIndex; } }
        public int GridResolution { get { return this._gridResolution; } }
        public List<string> GridFields { get { return this._gridFields; } }
        public string Compression { get { return this._compression; } }
        
        public byte[] Render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.Minx, envelope.Miny, envelope.Maxx, envelope.Maxy);

                format = format.ToLower();
                // Render Image
                if (format == "png" || format == "jpg")
                {
                    Image img = new Image(Convert.ToInt32(_map.Width), Convert.ToInt32(_map.Height));
                    _map.Render(img);
                    if (format == "png")
                    {
                        format = this._pngOptions;
                    }
                    if (format == "jpg")
                    {
                        format = this._jpegOptions;
                    }
                    return img.Encode(format);
                }

                // Render UTFGrid
                else if (format == "json")
                {
                    NETMapnik.Grid grd = new NETMapnik.Grid(_map.Width, _map.Height);
                    _map.Render(grd, Convert.ToUInt32(this._gridLayerIndex), this._gridFields);
                    string json = JsonConvert.SerializeObject(grd.Encode("utf", true, Convert.ToUInt32(this._gridResolution)));
                    return Encoding.UTF8.GetBytes(json);
                }

                // Render vector tile
                else if (format == "pbf")
                {
                    //tile coord (i.e., 0/0/0 not needed for pbf rendering
                    VectorTile vTile = new VectorTile(0,0,0, _map.Width,_map.Height);
                    _map.Render(vTile);
                    byte[] bytes =  vTile.GetBytes();

                    //compress vector tile bytes
                    if (bytes.Length > 0)
                    {
                        bytes = Compress(bytes);
                    }
                    return bytes;
                }
                // Format not expected so throw exception
                throw new InvalidOperationException(string.Format("Format {0} not expected",format));
            }
        }

        public List<string> GetFormats()
        {
            return new List<string> { "png", "jpg", "json", "pbf" };
        }

        private byte[] Compress(byte[] bytes)
        {
            if (this._compression.ToLower() == "gzip")
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                    {
                        gzip.Write(bytes, 0, bytes.Length);
                    }
                    return memory.ToArray();
                }
            }
            if (this._compression.ToLower() == "deflate")
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (ZlibStream deflate = new ZlibStream(memory, CompressionMode.Compress, true))
                    {
                        deflate.Write(bytes, 0, bytes.Length);
                    }
                    return memory.ToArray();
                }
            }
            //no compression
            return bytes;
        }

        public static void RegisterDatasources(string path)
        {
            DatasourceCache.RegisterDatasources(path);
        }

        public static void RegisterFonts(string path)
        {
            FreetypeEngine.RegisterFonts(path, false);
        }

        public List<VectorLayerMetadata> GetVectorTileMetadata()
        {
            List<VectorLayerMetadata> layerMetadataList = new List<VectorLayerMetadata>();
            //Look for vector_layers in json paramater
            object json;
            //object json = @"{""vector_layers"":[{""id"":""world_merc"",""description"":"""",""fields"":{""FIPS"":""String"",""ISO2"":""String"",""ISO3"":""String"",""UN"":""Number"",""NAME"":""String"",""AREA"":""Number"",""POP2005"":""Number"",""REGION"":""Number"",""SUBREGION"":""Number"",""LON"":""Number"",""LAT"":""Number""}}]}";
            if (this._map.Parameters.TryGetValue("json", out json))
            {
                if (json is string)
                {
                    try
                    {
                        MapnikVectorTileMetadata mapnikTileMetadata = JsonConvert.DeserializeObject<MapnikVectorTileMetadata>((string)json);
                        foreach (MapnikVectorLayerMetadata mapnikLayerMetadata in mapnikTileMetadata.vector_layers)
                        {
                            VectorLayerMetadata layerMetadata = new VectorLayerMetadata();
                            layerMetadata.Name = mapnikLayerMetadata.id;
                            layerMetadata.Description = mapnikLayerMetadata.description;
                            layerMetadata.Fields = mapnikLayerMetadata.fields;
                            layerMetadataList.Add(layerMetadata);
                        }
                        return layerMetadataList;
                    }
                    catch { }
                }

            }
            //interograte _map object to build vector_layers?

            return layerMetadataList;
        }
    }

    
    public class MapnikVectorTileMetadata
    {
        public MapnikVectorTileMetadata() { }
        public List<MapnikVectorLayerMetadata> vector_layers { get; set; }

    }

    
    public class MapnikVectorLayerMetadata
    {
        public MapnikVectorLayerMetadata() { }
        public string id { get; set; }
        public string description { get; set; }
        public Dictionary<string, string> fields { get; set; }
    }

}
