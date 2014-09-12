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
            List<double> resolutions = new List<double>() {
                156543.03390625,
                78271.516953125, 
                39135.7584765625,
                19567.87923828125,
                9783.939619140625,
                4891.9698095703125,
                2445.9849047851562, 
                1222.9924523925781,
                611.4962261962891,
                305.74811309814453, 
                152.87405654907226, 
                76.43702827453613, 
                38.218514137268066,
                19.109257068634033, 
                9.554628534317017, 
                4.777314267158508, 
                2.388657133579254,
                1.194328566789627,
                0.5971642833948135,
                0.29858214169740677, 
                0.14929107084870338
            };

            return new GridSet(
                "SphericalMercator",
                "EPSG:900913",
                new Envelope(-20037508.34, -20037508.34, 20037508.34, 20037508.34),
                resolutions,
                256,
                256,
                .00028,
                false
            );
        }
    }
}
