using System;
using System.Collections.Generic;
using System.Text;
using NETMapnik;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class MapnikProvider : IProvider
    {
        
        private Map _map;

        private MapnikProvider() { }
        
        public MapnikProvider(string xmlConfig, string datasourcePath, string fontPath)
        {
            this.xmlConfig = xmlConfig;
            this.datasourcePath = datasourcePath;
            this.fontPath = fontPath;
            
            DatasourceCache.RegisterDatasources(datasourcePath);
            //register fonts

            _map = new Map();
            _map.load_map(xmlConfig);

        }

        [DataMember(IsRequired=true)]
        public string xmlConfig { get; set; }

        [DataMember(IsRequired = true)]
        public string datasourcePath { get; set; }

        [DataMember(IsRequired = true)]
        public string fontPath { get; set; }

        public byte[] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            _map.width =  Convert.ToUInt32(tileWidth);
            _map.height = Convert.ToUInt32(tileHeight);
            _map.zoom_to_box(envelope.minx, envelope.miny, envelope.maxx, envelope.maxy);
            return _map.save_to_bytes(format);
        }

        public List<string> getFormats()
        {
            return new List<string>{"png", "jpg"};
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            DatasourceCache.RegisterDatasources(datasourcePath);
            //register fonts

            _map = new Map();
            _map.load_map(xmlConfig);
        }
    }
}
