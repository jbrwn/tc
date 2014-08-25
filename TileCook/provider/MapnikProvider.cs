using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NETMapnik;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TileCook
{
    [DataContract]
    public class MapnikProvider : IEnvelopeProvider
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

        public byte[] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.minx, envelope.miny, envelope.maxx, envelope.maxy);
                _map.Buffer = this.Buffer;

                format = format.ToLower();
                byte[] bytes = null;
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
                    bytes = img.Encode(format);
                }

                // Render UTFGrid
                if (format == "json")
                {
                    NETMapnik.Grid grd = new NETMapnik.Grid(_map.Width, _map.Height);
                    _map.Render(grd, Convert.ToUInt32(this.gridLayerIndex), this.gridFields);
                    string json = JsonConvert.SerializeObject(grd.Encode("utf", true, Convert.ToUInt32(this.gridResolution)));
                    bytes =  Encoding.UTF8.GetBytes(json);
                }

                // Render vector tile
                if (format == "pbf")
                {
                    //tile coord (i.e., 0/0/0 not needed for pbf rendering
                    VectorTile vTile = new VectorTile(0,0,0, _map.Width,_map.Height);
                    _map.Render(vTile);
                    bytes =  vTile.GetBytes();
                }
                return bytes;
            }
        }

        public List<string> getFormats()
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
            
            if (!Path.IsPathRooted(this.xmlConfig))
            {
                xmlConfig = Path.Combine(LayerCache.ConfigDirectory,this.xmlConfig);
            }
            _map = new Map();
            _map.LoadMap(this.xmlConfig);
        }

        public static void RegisterDatasources(string path)
        {
            DatasourceCache.RegisterDatasources(path);
        }

        public static void RegisterFonts(string path)
        {
            FreetypeEngine.RegisterFonts(path, false);
        }
    }
}
