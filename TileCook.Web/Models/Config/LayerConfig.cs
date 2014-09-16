using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TileCook.Web.Models.Config
{
    [DataContract]
    public class LayerConfig
    {
        public LayerConfig() { }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public CacheConfig Cache { get; set; }
        [DataMember]
        public ProviderConfig Provider { get; set; }
        [DataMember]
        public GridSetConfig Gridset { get; set; }
        [DataMember]
        public EnvelopeConfig Bounds { get; set; }
        [DataMember]
        public int MinZoom { get; set; }
        [DataMember]
        public int MaxZoom { get; set; }
        [DataMember]
        public IList<string> Formats { get; set; }
        [DataMember]
        public int BrowserCache { get; set; }
        [DataMember]
        public bool DisableCache { get; set; }
        [DataMember]
        public bool DisableProvider { get; set; }
    }
}