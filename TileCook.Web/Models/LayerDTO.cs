using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TileCook.Web.Models
{
    [DataContract]
    public class LayerDTO
    {
        public LayerDTO() { }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public CacheDTO Cache { get; set; }
        [DataMember]
        public ProviderDTO Provider { get; set; }
        [DataMember(IsRequired = true)]
        public GridSetDTO Gridset { get; set; }
        [DataMember]
        public EnvelopeDTO Bounds { get; set; }
        [DataMember]
        public int MinZoom { get; set; }
        [DataMember]
        public int MaxZoom { get; set; }
        [DataMember]
        public List<string> Formats { get; set; }
        [DataMember]
        public int BrowserCache { get; set; }
        [DataMember]
        public bool DisableCache { get; set; }
        [DataMember]
        public bool DisableProvider { get; set; }
    }

    public class CacheDTO
    {
        public CacheDTO() { }

        [DataMember(IsRequired = true)]
        public string Type { get; set; }

        // DiskCache
        [DataMember]
        public string CacheDirectory { get; set; }

        // Mbtiles cache
        [DataMember]
        public string Database { get; set; }
        [DataMember]
        public string Format { get; set; }
    }

    public class ProviderDTO
    {
        public ProviderDTO() { }

        [DataMember(IsRequired = true)]
        public string type { get; set; }

        // mapnik provider
        [DataMember]
        public string XmlConfig { get; set; }
        [DataMember]
        public int Buffer { get; set; }
        [DataMember]
        public string PngOptions { get; set; }
        [DataMember]
        public string JpegOptions { get; set; }
        [DataMember]
        public int GridLayerIndex { get; set; }
        [DataMember]
        public List<string> GridFields { get; set; }
        [DataMember]
        public int GridResolution { get; set; }
        [DataMember]
        public string Compression { get; set; }

        // mapnik vector tile provider
        [DataMember]
        public string TileSource { get; set; }

        // proxy provider
        [DataMember]
        public string UrlTemplate { get; set; }

        // WMS provider
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Layers { get; set; }
        [DataMember]
        public string CRS { get; set; }
        [DataMember]
        public string Styles { get; set; }
        [DataMember]
        public string Format { get; set; }
        [DataMember]
        public string SLD { get; set; }
        [DataMember]
        public string Bgcolor { get; set; }
        [DataMember]
        public string Transparent { get; set; }
    }

    public class GridSetDTO
    {
        public GridSetDTO() { }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }
        [DataMember]
        public string SRS { get; set; }
        [DataMember]
        public double MetersPerUnit { get; set; }
        [DataMember]
        public EnvelopeDTO Envelope { get; set; }
        [DataMember]
        public int TileSize { get; set; }
        [DataMember]
        public int ZoomLevels { get; set; }
        [DataMember]
        public bool TopOrigin { get; set; }
    }

    public class EnvelopeDTO
    {
        public EnvelopeDTO() { }
        [DataMember(IsRequired = true)]
        public double Minx { get; set; }
        [DataMember(IsRequired = true)]
        public double Miny { get; set; }
        [DataMember(IsRequired = true)]
        public double Maxx { get; set; }
        [DataMember(IsRequired = true)]
        public double Maxy { get; set; }
    }

}