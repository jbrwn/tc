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
        private string _name = "test gridset";
        private string _srs = "epsg:900913";
        private Envelope _envelope = new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789);
        private int _tileSize = 256;
        private int _levels = 18;
        private double _metersPerUnit = 1;
        private bool _topOrigin = false;

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

        public static implicit operator GridSet(GridSetBuilder obj)
        {
            return obj.Build();
        }
    }
}
