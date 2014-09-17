using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TileCook.API.Models
{
    //https://github.com/mapbox/tilejson-spec/tree/master/2.1.0

    [DataContract]
    public class TileJSON
    {
        public TileJSON() {}

        [DataMember(IsRequired = true)]
        public string tilejson;
        [DataMember(EmitDefaultValue = false)]
        public string name;
        [DataMember(EmitDefaultValue = false)]
        public string descritpion;
        [DataMember(EmitDefaultValue = false)]
        public string version;
        [DataMember(EmitDefaultValue = false)]
        public string attribution;
        [DataMember(EmitDefaultValue = false)]
        public string template;
        [DataMember(EmitDefaultValue = false)]
        public string legend;
        [DataMember(EmitDefaultValue = false)]
        public string scheme;
        [DataMember(IsRequired = true)]
        public List<string> tiles;
        [DataMember(EmitDefaultValue = false)]
        public List<string> grids;
        [DataMember(EmitDefaultValue = false)]
        public List<string> data;
        [DataMember]
        public int minzoom;
        [DataMember(EmitDefaultValue = false)]
        public int maxzoom;
        [DataMember(EmitDefaultValue = false)]
        public List<int> bounds;
        [DataMember(EmitDefaultValue = false)]
        public List<int> center;
        [DataMember(EmitDefaultValue = false)]
        public List<vector_layer> vector_layers;
    }

    [DataContract]
    public class vector_layer
    {
        public vector_layer() { }

        [DataMember(IsRequired = true)]
        public string id;
        [DataMember(IsRequired = true)]
        public string descritpion;
        [DataMember(IsRequired = true)]
        public Dictionary<string, string> fields;
    }
}