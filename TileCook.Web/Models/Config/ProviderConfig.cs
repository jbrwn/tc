using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Web.Models.Config
{

    public class ProviderConfig
    {
        public ProviderConfig() { }

        [DataMember(IsRequired = true)]
        public string Type { get; set; }

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
        public IEnumerable<string> GridFields { get; set; }
        [DataMember]
        public int GridResolution { get; set; }
        [DataMember]
        public string Compression { get; set; }

        // mapnik vector tile provider
        [DataMember]
        public LayerConfig TileSource { get; set; }

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

}