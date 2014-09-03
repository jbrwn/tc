using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace TileCook
{
    [DataContract]
    public class GridSet
    {
        
        private const double METER_PER_PIXEL = .00028;  //.28 mm

        private GridSet() {}

        public GridSet(string wellKnownScaleSet)
        {
            this.CopyFrom(WellKnownScaleSet.GetGridSet(wellKnownScaleSet));

        }

        public GridSet(string name, string srs, Envelope envelope, int zoomLevels, int tileSize, double metersPerUnit, bool topOrigin)
        {
            this.Name = name;
            this.SRS = srs;
            this.Envelope = envelope;
            this.TileWidth = tileSize;
            this.TileHeight = tileSize;
            this.MetersPerUnit = metersPerUnit;
            this.TopOrigin = topOrigin;

            this.Grids = new List<Grid>();
            double initialResolution = (envelope.Maxx - envelope.Minx) / tileSize;
            for (int i = 0; i < zoomLevels; i++)
            {
                Grid g = new Grid();
                g.Name = i.ToString();
                g.Scale = (initialResolution / Math.Pow(2, i)) / METER_PER_PIXEL * metersPerUnit;
                this.Grids.Add(g);
            }
        }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember]
        public string SRS { get; set; }

        [DataMember]
        public double MetersPerUnit { get; set; }

        [DataMember]
        public Envelope Envelope { get; set; }

        [DataMember]
        public List<Grid> Grids { get; set; }

        [DataMember]
        public int TileWidth { get; set; }

        [DataMember]
        public int TileHeight { get; set; }

        [DataMember]
        public bool TopOrigin { get; set; }

        public double Resolution(int z)
        {
            return this.Grids[z].Scale * METER_PER_PIXEL / this.MetersPerUnit;
        }

        public int GridWidth(int z)
        {
            double res = this.Resolution(z);
            return (int)Math.Ceiling((this.Envelope.Maxx - this.Envelope.Minx) / res / this.TileWidth);
            
        }

        public int GridHeight(int z)
        {
            double res = this.Resolution(z);
            return (int)Math.Ceiling((this.Envelope.Maxy - this.Envelope.Miny) / res / this.TileHeight);
        }

        public Envelope CoordToEnvelope(Coord coord)
        {
            if (this.TopOrigin)
            {
                coord.Y = this.GridHeight(coord.Z) - coord.Y - 1;
            }

            double res = this.Resolution(coord.Z);
            double minx = coord.X * this.TileWidth * res + this.Envelope.Minx;
            double maxx = (coord.X + 1) * this.TileWidth * res + this.Envelope.Minx;
            double miny = coord.Y * this.TileHeight * res + this.Envelope.Miny;
            double maxy = (coord.Y + 1) * this.TileHeight * res + this.Envelope.Miny;

            return new Envelope(minx, miny, maxx, maxy);

        }

        public Coord PointToCoord(Point p , int z)
        {
            double res = this.Resolution(z);
            int x =  (int)Math.Ceiling((p.X - this.Envelope.Minx) / res / this.TileWidth);
            int y = (int)Math.Ceiling((p.Y - this.Envelope.Miny) / res / this.TileHeight);
            return new Coord(z,x,y);
        }

        private void CopyFrom(GridSet other)
        {
            this.Name = other.Name;
            this.SRS = other.SRS;
            this.Envelope = other.Envelope;
            this.TileWidth = other.TileWidth;
            this.TileHeight = other.TileHeight;
            this.MetersPerUnit = other.MetersPerUnit;
            this.Grids = other.Grids;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (
                this.Name != null &&
                this.SRS == null &&
                this.Envelope == null &&
                this.TileWidth == 0 &&
                this.TileHeight == 0 &&
                this.MetersPerUnit == 0 &&
                this.Grids == null
            )
            {
                this.CopyFrom(WellKnownScaleSet.GetGridSet(this.Name));
            }

            //validate properties
            if (
                this.Name == null ||
                this.SRS == null ||
                this.Envelope == null ||
                this.TileWidth == 0 ||
                this.TileHeight == 0 ||
                this.MetersPerUnit == 0 ||
                this.Grids == null
            )
            {
                throw new SerializationException();
            }
        }
    }
}
