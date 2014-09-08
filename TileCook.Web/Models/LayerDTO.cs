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
        public IEnumerable<string> Formats { get; set; }
        [DataMember]
        public int BrowserCache { get; set; }
        [DataMember]
        public bool DisableCache { get; set; }
        [DataMember]
        public bool DisableProvider { get; set; }
    }
}