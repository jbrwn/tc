using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public class Coord
    {
        private int _z;
        private int _x;
        private int _y;

        public Coord(int z, int x, int y)
        {
            this._z = z;
            this._x = x;
            this._y = y;
        }

        public int Z { get { return this._z; } }
        public int X { get { return this._x; } }
        public int Y { get { return this._y; } }
    }
}
