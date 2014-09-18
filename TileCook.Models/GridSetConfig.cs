using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TileCook.Models
{
    [DataContract]
    public class GridSetConfig
    {
        public GridSetConfig() 
        {
            this.TileSize = 256;
            this.Levels = 18;
            this.Step = 2;
            this.PixelSize = .00028;
            this.TopOrigin = false;
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SRS { get; set; }
        [DataMember]
        public EnvelopeConfig Envelope { get; set; }
        [DataMember]
        public int TileSize { get; set; }
        [DataMember]
        public int Levels { get; set; }
        [DataMember]
        public int Step { get; set; }
        [DataMember]
        public double PixelSize { get; set; }
        [DataMember]
        public bool TopOrigin { get; set; }
    }
}