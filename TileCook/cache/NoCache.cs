using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace TileCook
{
    [DataContract]
    public class NoCache : ICache
    {
        public NoCache() { }
        
        public byte[] get(int z, int x, int y, string format)
        {
            return null;
        }

        public void put(int z, int x, int y, string format, byte[] image)
        {
        }

        public void delete(int z, int x, int y, string format)
        {
        }
    }
}
