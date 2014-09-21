using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileCook
{
    public class GridSet : IGridSet
    {
        private readonly string _name;
        private readonly string _srs;
        private readonly Envelope _envelope;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        private readonly double _pixelSize;
        private readonly bool _topOrigin;
        private readonly List<double> _resolutions;

        public GridSet(string name, string srs, Envelope envelope, IList<double> resolutions, int tileWidth = 256, int tileHeight = 256, double pixelSize = .00028, bool topOrigin = false)
        {
            // Set name
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("GridSet Name cannot be null or empty");
            this._name = name;
            
            // set SRS
            if (string.IsNullOrEmpty(srs))
                throw new ArgumentNullException("GridSet SRS cannot be null or empty");
            this._srs = srs;

            // set Envelope
            if (envelope == null)
                throw new ArgumentNullException("GridSet Envelope cannot be null");
            this._envelope = envelope;

            // Set resolutions list
            if (resolutions == null)
                throw new ArgumentNullException("GridSet Resolutions cannot be null");
            this._resolutions = resolutions as List<double>;
            
            // Set tile sizes
            if (tileWidth <= 0)
                throw new ArgumentOutOfRangeException("GridSet TileWidth must be greater than 0");
            this._tileWidth = tileWidth;
            if (tileHeight <= 0)
                throw new ArgumentOutOfRangeException("GridSet TileHeight must be greater than 0");
            this._tileHeight = tileHeight;

            // Set pixelSize
            if (pixelSize <= 0)
                throw new ArgumentOutOfRangeException("PixelSize must be greater than 0");
            this._pixelSize = pixelSize;

            // Set top orgin
            this._topOrigin = topOrigin;
        }

        public string Name { get { return this._name; } }
        public string SRS { get { return this._srs; } }
        public double PixelSize { get { return this._pixelSize;} }
        public Envelope Envelope { get { return this._envelope; } }
        public IList<double> Resolutions { get { return new List<double>(this._resolutions); } }
        public int TileWidth { get { return this._tileWidth; } }
        public int TileHeight { get { return this._tileHeight; } }
        public bool TopOrigin { get { return this._topOrigin; } }

        public double Scale(int z)
        {
            return this._resolutions[z] / this._pixelSize;
        }

        public int GridWidth(int z)
        {
            double res = this._resolutions[z];
            return (int)Math.Ceiling((this._envelope.Maxx - this._envelope.Minx) / res / this._tileWidth);
        }

        public int GridHeight(int z)
        {
            double res = this._resolutions[z];
            return (int)Math.Ceiling((this._envelope.Maxy - this._envelope.Miny) / res / this._tileHeight);
        }

        public Envelope CoordToEnvelope(Coord coord)
        {
            int z = coord.Z;
            int x = coord.X;
            int y = coord.Y;

            if (coord.TopOrigin)
            {
                y = this.GridHeight(z) - y - 1;
            }

            if (!(z >= 0 && z < this._resolutions.Count))
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", coord.Z, 0, this._resolutions.Count - 1));
            int xMax = GridWidth(z);
            if (!(x >=0 && x < xMax))
                throw new TileOutOfRangeException(string.Format("Column {0} is out of range (min: {1} max: {2})", coord.X, 0, xMax - 1));
            int yMax = GridHeight(z);
            if (!(y >=0 && y < yMax))
                throw new TileOutOfRangeException(string.Format("Row {0} is out of range (min: {1} max: {2})", coord.Y, 0, yMax - 1));

            double res = this._resolutions[z];
            double minx = x * this._tileWidth * res + this._envelope.Minx;
            double maxx = (x + 1) * this._tileWidth * res + this._envelope.Minx;
            double miny = y * this._tileHeight * res + this._envelope.Miny;
            double maxy = (y + 1) * this._tileHeight * res + this._envelope.Miny;

            return new Envelope(minx, miny, maxx, maxy);
        }

        public Coord PointToCoord(Point p , int z)
        {
            double res = this._resolutions[z];
            int x =  (int)Math.Ceiling((p.X - this._envelope.Minx) / res / this._tileWidth);
            int y = (int)Math.Ceiling((p.Y - this._envelope.Miny) / res / this._tileHeight);
            return new Coord(z,x,y);
        }

        public Coord SetY(Coord coord)
        {
            if (this.TopOrigin != coord.TopOrigin)
                return FlipY(coord);

            return coord;
        }

        private Coord FlipY(Coord coord)
        {
            int y = this.GridHeight(coord.Z) - coord.Y - 1;
            return new Coord(coord.Z, coord.X, y, !coord.TopOrigin);
        }
    }
}
