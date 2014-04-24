using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TileCook.Web.TMSService
{
    [XmlRoot("Services")]
    public class Services : IEnumerable<TileMapServiceMetadata>
    {
        List<TileMapServiceMetadata> _services;
        public Services()
        {
            _services = new List<TileMapServiceMetadata>();
        }

        public void Add(TileMapServiceMetadata item)
        {
            this._services.Add(item);
        }
        
        public IEnumerator<TileMapServiceMetadata> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [XmlRoot("TileMapService")]
    public class TileMapServiceMetadata
    {
        public TileMapServiceMetadata() { }
        public TileMapServiceMetadata(string title, string version, string href)
        {
            this.title = title;
            this.version = version;
            this.href = href;
        }

        [XmlAttribute]
        public string title;
        [XmlAttribute]
        public string version;
        [XmlAttribute]
        public string href;
    }


}