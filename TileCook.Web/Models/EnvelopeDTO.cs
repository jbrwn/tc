using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Web.Models
{
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