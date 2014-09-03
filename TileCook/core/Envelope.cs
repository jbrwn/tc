using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class Envelope
    {
        private Envelope() { }

        public Envelope(double minx, double miny, double maxx, double maxy)
        {
            this.Minx = minx;
            this.Miny = miny;
            this.Maxx = maxx;
            this.Maxy = maxy;
        }

        [DataMember(IsRequired=true)]
        public double Minx { get; set; }

        [DataMember(IsRequired = true)]
        public double Miny { get; set; }

        [DataMember(IsRequired = true)]
        public double Maxx { get; set; }

        [DataMember(IsRequired = true)]
        public double Maxy { get; set; }

        public bool Equals(Envelope other)
        {
            if (this.Minx == other.Minx &&
                this.Miny == other.Miny &&
                this.Maxx == other.Maxx &&
                this.Maxy == other.Maxy)
            {
                return true;
            }
            return false;
        }

        
    }
}
