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
            this.minx = minx;
            this.miny = miny;
            this.maxx = maxx;
            this.maxy = maxy;
        }

        [DataMember(IsRequired=true)]
        public double minx { get; set; }

        [DataMember(IsRequired = true)]
        public double miny { get; set; }

        [DataMember(IsRequired = true)]
        public double maxx { get; set; }

        [DataMember(IsRequired = true)]
        public double maxy { get; set; }

        public bool Equals(Envelope other)
        {
            if (this.minx == other.minx &&
                this.miny == other.miny &&
                this.maxx == other.maxx &&
                this.maxy == other.maxy)
            {
                return true;
            }
            return false;
        }

        
    }
}
