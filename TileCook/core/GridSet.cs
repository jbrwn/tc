using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileCook
{
    
    public class GridSet : IGridSet
    {
        
        private const double METER_PER_PIXEL = .00028;  //.28 mm

        private string _name;
        private string _srs;
        private Envelope _envelope;
        private int _tileWidth;
        private int _tileHeight;
        private double _metersPerUnit;
        private bool _topOrigin;
        private List<Grid> _grids;

        //public GridSet(string wellKnownScaleSet)
        //{
        //    this.CopyFrom(WellKnownScaleSet.GetGridSet(wellKnownScaleSet));

        //}

        public GridSet(string name, string srs, Envelope envelope, int zoomLevels, int tileSize, double metersPerUnit, bool topOrigin)
        {
            // Set name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("GridSet name cannot be null or empty");
            }
            else
            {
                this._name = name;
            }
            
            // set SRS
            this._srs = srs;

            // set Envelope
            if (envelope == null)
            {
                throw new ArgumentNullException("GridSet Envelope cannot be null");
            }
            else
            {
                this._envelope = envelope;
            }

            // Set tile size
            this._tileWidth = tileSize;
            this._tileHeight = tileSize;

            // Set meters per unit
            this._metersPerUnit = metersPerUnit;

            // Set top orgin
            this._topOrigin = topOrigin;

            // Initialize grid list
            this._grids = new List<Grid>();
            double initialResolution = (envelope.Maxx - envelope.Minx) / tileSize;
            for (int i = 0; i < zoomLevels; i++)
            {
                
                string gridName = i.ToString();
                double gridScale = (initialResolution / Math.Pow(2, i)) / METER_PER_PIXEL * metersPerUnit;
                Grid g = new Grid(gridName, gridScale);
                this._grids.Add(g);
            }
        }

        public string Name { get { return this._name; } }
        public string SRS { get { return this._srs; } }
        public double MetersPerUnit { get { return this._metersPerUnit;} }
        public Envelope Envelope { get { return this._envelope; } }

        // TO DO: return deep copy clone
        public List<Grid> Grids { get { return new List<Grid>(this._grids); } }

        public int TileWidth { get { return this._tileWidth; } }
        public int TileHeight { get { return this._tileHeight; } }
        public bool TopOrigin { get { return this._topOrigin; } }

        public double Resolution(int z)
        {
            return this._grids[z].Scale * METER_PER_PIXEL / this.MetersPerUnit;
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
            int z = coord.Z;
            int x = coord.X;
            int y = coord.Y;

            if (this.TopOrigin)
            {
                y = this.GridHeight(z) - y - 1;
            }

            double res = this.Resolution(z);
            double minx = x * this.TileWidth * res + this.Envelope.Minx;
            double maxx = (x + 1) * this.TileWidth * res + this.Envelope.Minx;
            double miny = y * this.TileHeight * res + this.Envelope.Miny;
            double maxy = (y + 1) * this.TileHeight * res + this.Envelope.Miny;

            return new Envelope(minx, miny, maxx, maxy);

        }

        public Coord PointToCoord(Point p , int z)
        {
            double res = this.Resolution(z);
            int x =  (int)Math.Ceiling((p.X - this.Envelope.Minx) / res / this.TileWidth);
            int y = (int)Math.Ceiling((p.Y - this.Envelope.Miny) / res / this.TileHeight);
            return new Coord(z,x,y);
        }

        //private void CopyFrom(GridSet other)
        //{
        //    this.Name = other.Name;
        //    this.SRS = other.SRS;
        //    this.Envelope = other.Envelope;
        //    this.TileWidth = other.TileWidth;
        //    this.TileHeight = other.TileHeight;
        //    this.MetersPerUnit = other.MetersPerUnit;
        //    this.Grids = other.Grids;
        //}
    }
}
