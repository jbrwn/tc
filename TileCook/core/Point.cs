using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public class Point
    {
        private double _x;
        private double _y;

        public Point(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        public double X { get { return this._x; } }
        public double Y { get { return this._y; } }

    }
}
