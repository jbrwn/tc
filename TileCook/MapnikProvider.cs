using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NETMapnik;
using System.Runtime.Serialization;

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
            
            _map = new Map();
            _map.load_map(xmlConfig);
        }

        [DataMember(IsRequired=true)]
        public string xmlConfig { get; set; }

        public byte[] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.width = Convert.ToUInt32(tileWidth);
                _map.height = Convert.ToUInt32(tileHeight);
                _map.zoom_to_box(envelope.minx, envelope.miny, envelope.maxx, envelope.maxy);
                return _map.save_to_bytes(format);
            }
        }

        public List<string> getFormats()
        {
            return new List<string>{"png", "jpg"};
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (!Path.IsPathRooted(xmlConfig))
            {
                xmlConfig = Path.Combine(LayerCache.ConfigDirectory, xmlConfig);
            }
            _map = new Map();
            _map.load_map(xmlConfig);
        }

        public static void RegisterDatasources(string path)
        {
            DatasourceCache.RegisterDatasources(path);
        }

        public static void RegisterFonts(string path)
        {
            freetype_engine.RegisterFonts(path, false);
        }
    }
}
