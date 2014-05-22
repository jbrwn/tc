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
            this.z = z;
            this.x = x;
            this.y = y;
        }
        
        public int z { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
