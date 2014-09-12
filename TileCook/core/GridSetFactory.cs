using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public enum WellKnownGridSet
    {
        GoogleMapsCompatible,
        GlobalCRS84Scale,
        GlobalCRS84Pixel,
        GoogleCRS84Quad,
        SphericalMercator
    }

    public static class GridSetFactory
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
                case WellKnownGridSet.SphericalMercator:
                    g = SphericalMercator();
                    break;
            }
            return g;
        }

        public static IGridSet CreateGridSet(string name, string srs, Envelope envelope, int tileSize = 256, int levels= 18, double step = 2, double pixelSize = .00028, bool topOrigin = false)
        {
            if (envelope == null)
                throw new ArgumentNullException("GridSetFactory Envelope cannot be null");

            if (tileSize <= 0)
                throw new ArgumentOutOfRangeException("GridSetFactory TileSize must be greater than 0");

            if (step <= 0)
                throw new ArgumentOutOfRangeException("GridSetFactory Step must be greater than 0");

            List<double> resolutions = new List<double>();
            for (int i = 0; i < levels; i++)
            {
                double r = (envelope.Maxx - envelope.Minx) / tileSize / Math.Pow(step, i);
                resolutions.Add(r);
            }

            return new GridSet(
                name,
                srs,
                envelope,
                resolutions,
                tileSize,
                tileSize,
                pixelSize,
                topOrigin
            );
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

        private static IGridSet SphericalMercator()
        {
            throw new NotImplementedException();
        }
    }
}
