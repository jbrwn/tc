using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileCook
{
    public class Grid
    {
        private string _name;
        private double _scale;

        public Grid(string name, double scale)
        {
            this._name = name;
            this._scale = scale;
        }
        public string Name { get { return this._name; } }
        public double Scale { get { return this._scale; } }
    }
}
