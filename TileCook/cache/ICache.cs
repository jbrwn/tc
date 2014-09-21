using System;
using System.Collections.Generic;
using System.Text;

namespace TileCook
{
    
    public interface ICache
    {
        byte[] Get(Coord coord, string format);
        void Put(Coord coord, string format, byte[] image);
        void Delete(Coord coord, string format);
    }
}
