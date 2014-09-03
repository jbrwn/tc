using System;
using System.Collections.Generic;
using System.Text;

namespace TileCook
{
    
    public interface ICache
    {
        byte[] Get(int z, int x, int y, string format);
        void Put(int z, int x, int y, string format, byte[] image);
        void Delete(int z, int x, int y, string format);
    }
}
