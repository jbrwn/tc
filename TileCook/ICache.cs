using System;
using System.Collections.Generic;
using System.Text;

namespace TileCook
{
    
    public interface ICache
    {
        byte[] get(int z, int x, int y, string format);
        void put(int z, int x, int y, string format, byte[] image);
        void delete(int z, int x, int y, string format);
    }
}
