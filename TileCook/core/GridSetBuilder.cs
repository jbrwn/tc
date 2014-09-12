using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public class GridSetBuilder
    {
        private string _name;
        private string _srs;
        private Envelope _envelope;
        private int _tileSize;
        private int _levels;
        private double _step;
        private double _pixelSize;
        private bool _topOrigin;

        public GridSetBuilder()
        {
            // Set defaults
            this._tileSize = 256;
            this._levels = 18;
            this._step = 2;
            this._pixelSize = .00028;
            this._topOrigin = false;
        }

        public GridSet Build()
        {
            if (this._envelope == null)
                throw new ArgumentNullException("GridSetBuilder Envelope cannot be null");

            List<double> resolutions = new List<double>();
            for (int i = 0; i < this._levels; i++)
            {
                double r = (this._envelope.Maxx - this._envelope.Minx) / this._tileSize / Math.Pow(this._step, i);
                resolutions.Add(r);
            }
            return new GridSet(
                this._name,
                this._srs,
                this._envelope,
                resolutions,
                this._tileSize,
                this._tileSize,
                this._pixelSize,
                this._topOrigin
            );
        }

        public GridSetBuilder SetName(string name)
        {
            this._name = name;
            return this;
        }

        public GridSetBuilder SetSRS(string srs)
        {
            this._srs = srs;
            return this;
        }

        public GridSetBuilder SetEnvelope(Envelope envelope)
        {
            this._envelope = envelope;
            return this;
        }

        public GridSetBuilder SetTileSize(int tileSize)
        {
            this._tileSize = tileSize;
            return this;
        }

        public GridSetBuilder SetLevels(int levels)
        {
            this._levels = levels;
            return this;
        }

        public GridSetBuilder SetStep(double step)
        {
            this._step = step;
            return this;
        }

        public GridSetBuilder SetPixelSize(double pixelSize)
        {
            this._pixelSize = pixelSize;
            return this;
        }

        public GridSetBuilder SetTopOrigin(bool topOrigin)
        {
            this._topOrigin = topOrigin;
            return this;
        }

        public static implicit operator GridSet(GridSetBuilder obj)
        {
            return obj.Build();
        }

    }
}
