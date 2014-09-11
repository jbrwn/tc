using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Web.Models
{
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
}