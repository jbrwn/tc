using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public interface IPassThoughProvider : IProvider
    {
        byte[] Render(Coord coord, string format, int tileWidth, int tileHeight);
    }
}
