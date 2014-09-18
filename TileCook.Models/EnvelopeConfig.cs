using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Models
{
    public class EnvelopeConfig
    {
        public EnvelopeConfig() { }
        [DataMember]
        public double Minx { get; set; }
        [DataMember]
        public double Miny { get; set; }
        [DataMember]
        public double Maxx { get; set; }
        [DataMember]
        public double Maxy { get; set; }
    }
}