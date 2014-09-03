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
    [DataContract]
    public class MapnikProvider : IEnvelopeProvider,IVectorTileProvider
    {

        private Map _map;
        private static readonly Object mapLock = new Object();

        private MapnikProvider() { }
        
        public MapnikProvider(string xmlConfig)
        {
            this.xmlConfig = xmlConfig;
            this.Buffer = 0;

            //set img defaults
            this.pngOptions = "png";
            this.jpegOptions = "jpeg";

            //set grid defaults
            this.gridLayerIndex = 0;
            this.gridResolution = 4;
            this.gridFields = new List<string>();


            _map = new Map();
            _map.LoadMap(xmlConfig);
        }

        [DataMember(IsRequired=true)]
        public string xmlConfig { get; set; }

        [DataMember]
        public int Buffer { get; set; }

        [DataMember]
        public string pngOptions { get; set; }

        [DataMember]
        public string jpegOptions { get; set; }

        [DataMember]
        public int gridLayerIndex { get; set; }

        [DataMember]
        public List<string> gridFields { get; set; }

        [DataMember]
        public int gridResolution { get; set; }

        [DataMember]
        public string Compression { get; set; }

        public byte[] Render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.Minx, envelope.Miny, envelope.Maxx, envelope.Maxy);
                _map.Buffer = this.Buffer;

                format = format.ToLower();
                // Render Image
                if (format == "png" || format == "jpg")
                {
                    Image img = new Image(Convert.ToInt32(_map.Width), Convert.ToInt32(_map.Height));
                    _map.Render(img);
                    if (format == "png")
                    {
                        format = this.pngOptions;
                    }
                    if (format == "jpg")
                    {
                        format = this.jpegOptions;
                    }
                    return img.Encode(format);
                }

                // Render UTFGrid
                else if (format == "json")
                {
                    NETMapnik.Grid grd = new NETMapnik.Grid(_map.Width, _map.Height);
                    _map.Render(grd, Convert.ToUInt32(this.gridLayerIndex), this.gridFields);
                    string json = JsonConvert.SerializeObject(grd.Encode("utf", true, Convert.ToUInt32(this.gridResolution)));
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
            return new List<string>{"png", "jpg", "json", "pbf"};
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (this.jpegOptions == null) { this.jpegOptions = "jpeg"; }
            if (this.pngOptions == null) { this.pngOptions = "png"; }
            if (this.gridResolution == 0) { this.gridResolution = 4; }
            if (this.gridFields == null) { this.gridFields = new List<string>(); }
            //gridLayerIndex defaults to 0
            //buffer defaults to 0
            if (this.Compression == null) { this.Compression = "gzip"; }
            
            if (!Path.IsPathRooted(this.xmlConfig))
            {
                xmlConfig = Path.Combine(LayerCache.ConfigDirectory,this.xmlConfig);
            }
            _map = new Map();
            _map.LoadMap(this.xmlConfig);
        }

        private byte[] Compress(byte[] bytes)
        {
            if (this.Compression.ToLower() == "gzip")
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
            if (this.Compression.ToLower() == "deflate")
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

    [DataContract]
    public class MapnikVectorTileMetadata
    {
        public MapnikVectorTileMetadata() { }

        [DataMember]
        public List<MapnikVectorLayerMetadata> vector_layers { get; set; }

    }

    [DataContract]
    public class MapnikVectorLayerMetadata
    {
        public MapnikVectorLayerMetadata() { }

        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public Dictionary<string, string> fields { get; set; }
    }

}
