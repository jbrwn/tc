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
            this.name = name;
            this.srs = srs;
            this.envelope = envelope;
            this.tileWidth = tileSize;
            this.tileHeight = tileSize;
            this.metersPerUnit = metersPerUnit;
            this.topOrigin = topOrigin;

            this.grids = new List<Grid>();
            double initialResolution = (envelope.Maxx - envelope.Minx) / tileSize;
            for (int i = 0; i < zoomLevels; i++)
            {
                Grid g = new Grid();
                g.name = i.ToString();
                g.scale = (initialResolution / Math.Pow(2, i)) / METER_PER_PIXEL * metersPerUnit;
                this.grids.Add(g);
            }
        }

        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember]
        public string srs { get; set; }

        [DataMember]
        public double metersPerUnit { get; set; }

        [DataMember]
        public Envelope envelope { get; set; }

        [DataMember]
        public List<Grid> grids { get; set; }

        [DataMember]
        public int tileWidth { get; set; }

        [DataMember]
        public int tileHeight { get; set; }

        [DataMember]
        public bool topOrigin { get; set; }

        public double resolution(int z)
        {
            return grids[z].scale * METER_PER_PIXEL / metersPerUnit;
        }

        public int gridWidth(int z)
        {
            double res = resolution(z);
            return (int)Math.Ceiling((envelope.Maxx - envelope.Minx) / res / tileWidth);
            
        }

        public int gridHeight(int z)
        {
            double res = resolution(z);
            return (int)Math.Ceiling((envelope.Maxy - envelope.Miny) / res / tileHeight);
        }

        public Envelope CoordToEnvelope(Coord coord)
        {
            if (this.topOrigin)
            {
                coord.Y = gridHeight(coord.Z) - coord.Y - 1;
            }

            double res = resolution(coord.Z);
            double minx = coord.X * tileWidth * res + envelope.Minx;
            double maxx = (coord.X + 1) * tileWidth * res + envelope.Minx;
            double miny = coord.Y * tileHeight * res + envelope.Miny;
            double maxy = (coord.Y + 1) * tileHeight * res + envelope.Miny;

            return new Envelope(minx, miny, maxx, maxy);

        }

        public Coord PointToCoord(Point p , int z)
        {
            double res = resolution(z);
            int x =  (int)Math.Ceiling((p.X - envelope.Minx) / res / tileWidth);
            int y = (int)Math.Ceiling((p.Y - envelope.Miny) / res / tileHeight);
            return new Coord(z,x,y);
        }

        private void CopyFrom(GridSet other)
        {
            this.name = other.name;
            this.srs = other.srs;
            this.envelope = other.envelope;
            this.tileWidth = other.tileWidth;
            this.tileHeight = other.tileHeight;
            this.metersPerUnit = other.metersPerUnit;
            this.grids = other.grids;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (
                this.name != null &&
                this.srs == null &&
                this.envelope == null &&
                this.tileWidth == 0 &&
                this.tileHeight == 0 &&
                this.metersPerUnit == 0 &&
                this.grids == null
            )
            {
                this.CopyFrom(WellKnownScaleSet.GetGridSet(name));
            }

            //validate properties
            if (
                this.name == null ||
                this.srs == null ||
                this.envelope == null ||
                this.tileWidth == 0 ||
                this.tileHeight == 0 ||
                this.metersPerUnit == 0 ||
                this.grids == null
            )
            {
                throw new SerializationException();
            }
        }


    }

}
