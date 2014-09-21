using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TileCook.Models
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
    }
}