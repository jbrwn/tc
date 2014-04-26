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

        [XmlAttribute]
        public string title;
        [XmlAttribute]
        public string version;
        [XmlAttribute]
        public string href;
    }

    public class TileMapService
    {
        public TileMapService()
        {
        }
        
        [XmlAttribute]
        public string version;
        [XmlAttribute]
        public string services;

        public string Title;
        public string Abstract;
        public string KeywordList;
        public ContactInformation ContactInformation;

        [XmlArrayItem("TileMap")]
        public List<TileMapMetadata> TileMaps;

    }

    public class ContactInformation
    {
        public ContactInformation()
        {
        }

        public ContactPersonPrimary ContactPersonPrimary;
        public string ContactPosition;
        public ContactAddress ContactAddress;
        public string ContactVoiceTelephone;
        public string ContactFacsimileTelephone;
        public string ContactElectronicMailAddress;

    }

    public class ContactPersonPrimary
    {
        public ContactPersonPrimary()
        {
        }
        public string ContactPerson;
        public string ContactOrganization;
    }

    public class ContactAddress
    {
        public ContactAddress()
        {

        }
        
        public string AddressType;
        public string Address;
        public string City;
        public string StateOrProvince;
        public string PostCode;
        public string Country;
    }

    public class TileMapMetadata
    {
        public TileMapMetadata()
        {
        }
        
        [XmlAttribute]
        public string title;
        [XmlAttribute]
        public string srs;
        [XmlAttribute]
        public string profile;
        [XmlAttribute]
        public string href;
    }

    public class TileMap
    {
        public TileMap()
        {
        }

        [XmlAttribute]
        public string version;
        [XmlAttribute]
        public string tilemapservice;

        public string Title;
        public string Abstract;
        public string KeywordList;
        public Metadata Metadata;
        public Attribution Attribution;
        public WebMapContext WebMapContext;
        public string Face;
        public string SRS;
        public BoundingBox BoudningBox;
        public Origin Origin;
        public TileFormat TileFormat;
        public TileSets TileSets;
    }

    public class WebMapContext
    {
        public WebMapContext() { }

        [XmlAttribute]
        public string href;
    }

    public class Metadata
    {
        public Metadata() { }

        [XmlAttribute]
        public string type;
        [XmlAttribute("mime-type")]
        public string mimetype;
        [XmlAttribute]
        public string href;

    }

    public class Attribution
    {
        public Attribution() { }
        
        public string Title;
        public Logo Logo;
    }

    public class Logo
    {
        public Logo() { }
        
        [XmlAttribute]
        public string width;
        [XmlAttribute]
        public string height;
        [XmlAttribute]
        public string href;
        [XmlAttribute("mime-type")]
        public string mimetype;
    }

    public class BoundingBox
    {
        public BoundingBox() { }
        
        [XmlAttribute]
        public string minx;
        [XmlAttribute]
        public string miny;
        [XmlAttribute]
        public string maxx;
        [XmlAttribute]
        public string maxy;
    }

    public class Origin
    {
        public Origin() { }

        [XmlAttribute]
        public string x;
        [XmlAttribute]
        public string y;
    }

    public class TileFormat
    {
        public TileFormat() { }
        
        [XmlAttribute]
        public string width;
        [XmlAttribute]
        public string height;
        [XmlAttribute("mime-type")]
        public string mimetype;
        [XmlAttribute]
        public string extension;
    }

    [XmlRoot("TileSets")]
    public class TileSets : IEnumerable<TileSet>
    {
        List<TileSet> _tilesets;
        
        public TileSets()
        {
            this._tilesets = new List<TileSet>();
        }

        [XmlAttribute]
        public string profile;

        public void Add(TileSet item)
        {
            this._tilesets.Add(item);
        }

        public IEnumerator<TileSet> GetEnumerator()
        {
            return this._tilesets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TileSet
    {
        [XmlAttribute]
        public string href;
        [XmlAttribute("units-per-pixel")]
        public string unitsperpixel;
        [XmlAttribute]
        public string order;
    }

    public class TileMapServiceError
    {
        public TileMapServiceError() { }
        public string Message;
    }
}