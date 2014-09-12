using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.core
{
    public enum WellKnownGridSet
    {
        GoogleMapsCompatible,
        GlobalCRS84Scale,
        GlobalCRS84Pixel,
        GoogleCRS84Quad,
    }

    public static class WellKnownGridSetFactory
    {
        public static IGridSet CreateGridSet(WellKnownGridSet wkgs) 
        {
            IGridSet g = null;
            switch (wkgs)
            {
                case WellKnownGridSet.GoogleMapsCompatible:
                    g = GoogleMapsCompatible();
                    break;
                case WellKnownGridSet.GoogleCRS84Quad:
                    g = GoogleCRS84Quad();
                    break;
                case WellKnownGridSet.GlobalCRS84Scale:
                    g = GlobalCRS84Scale();
                    break;
                case WellKnownGridSet.GlobalCRS84Pixel:
                    g = GlobalCRS84Pixel();
                    break;
            }
            return g;
        }

        private static IGridSet GoogleMapsCompatible()
        {
            throw new NotImplementedException();
        }

        private static IGridSet GoogleCRS84Quad()
        {
            throw new NotImplementedException();
        }

        private static IGridSet GlobalCRS84Scale()
        {
            throw new NotImplementedException();
        }

        private static IGridSet GlobalCRS84Pixel()
        {
            throw new NotImplementedException();
        }
    }
}
