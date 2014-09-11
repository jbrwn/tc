using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Test
{
    // default paramaters return spherical mercator grid
    // 18 zoom levels
    // bottom left origin
    public class GridSetBuilder
    {
        private string _name;
        private string _srs;
        private Envelope _envelope;
        private int _tileSize;
        private int _levels;
        private double _metersPerUnit;
        private bool _topOrigin;

        public GridSet Build()
        {
            return new GridSet(
                this._name,
                this._srs,
                this._envelope,
                this._levels,
                this._tileSize,
                this._metersPerUnit,
                this._topOrigin
            );
        }

        public GridSetBuilder SphericalMercator()
        {
            this._name = "test gridset";
            this._srs = "epsg:900913";
            this._envelope = new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789);
            this._tileSize = 256;
            this._levels = 18;
            this._metersPerUnit = 1;
            this._topOrigin = false;
            return this;
        }

        public GridSetBuilder SetMetersPerUnit(int metersPerUnit)
        {
            this._metersPerUnit = metersPerUnit;
            return this;
        }

        public static implicit operator GridSet(GridSetBuilder obj)
        {
            return obj.Build();
        }
    }
}
