using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public class Coord
    {
        public Coord(int z, int x, int y)
        {
            this.Z = z;
            this.X = x;
            this.Y = y;
        }
        
        public int Z { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
