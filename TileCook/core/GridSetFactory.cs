using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.core
{
    public static class GridSetFactory
    {

        public static IGridSet CreateGridSet() 
        {
            throw new NotImplementedException();
        }

        public static IGridSet CreateGridSet(string name, string srs, Envelope envelope, int tileSize)
        {
            return CreateGridSet(name, srs, envelope, tileSize, 0, false);
        }

        public static IGridSet CreateGridSet(string name, string srs, Envelope envelope, int tileSize, int pixelSize, bool topOrigin)
        {
            throw new NotImplementedException();
        }

        private IGridSet GoogleMapsCompatible()
        {
            throw new NotImplementedException();
        }
    }
}
