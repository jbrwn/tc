using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileCook
{
    
    public class Envelope
    {
        private double _minx;
        private double _miny;
        private double _maxx;
        private double _maxy;

        public Envelope(double minx, double miny, double maxx, double maxy)
        {
            this._minx = minx;
            this._miny = miny;
            this._maxx = maxx;
            this._maxy = maxy;
        }

        public double Minx { get { return this._minx; } }
        public double Miny { get { return this._miny; } }
        public double Maxx { get { return this._maxx; } }
        public double Maxy { get { return this._maxy; } }

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

        public bool intersects(Envelope other)
        {
            if (this._minx <= other.Maxx &&
                this._maxx >= other.Minx &&
                this._miny <= other.Maxy &&
                this._maxy >= other.Miny)
            {
                return true;
            }
            return false;
        }
    }
}
