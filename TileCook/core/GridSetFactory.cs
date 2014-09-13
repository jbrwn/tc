using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public enum WellKnownGridSet
    {
        GOOGLE_MAPS_COMPATIBLE,
        GLOBAL_CRS84_SCALE,
        GLOBAL_CRS84_PIXEL,
        GOOGLE_CRS84_QUAD,
        SHPERICAL_MERCATOR
    }

    public static class GridSetFactory
    {
        public static IGridSet CreateGridSet(WellKnownGridSet wkgs)
        {
            IGridSet g = null;
            switch (wkgs)
            {
                case WellKnownGridSet.GOOGLE_MAPS_COMPATIBLE:
                    g = GOOGLE_MAPS_COMPATIBLE();
                    break;
                case WellKnownGridSet.GOOGLE_CRS84_QUAD:
                    g = GOOGLE_CRS84_QUAD();
                    break;
                case WellKnownGridSet.GLOBAL_CRS84_SCALE:
                    g = GLOBAL_CRS84_SCALE();
                    break;
                case WellKnownGridSet.GLOBAL_CRS84_PIXEL:
                    g = GLOBAL_CRS84_PIXEL();
                    break;
                case WellKnownGridSet.SHPERICAL_MERCATOR:
                    g = SHPERICAL_MERCATOR();
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

        private static IGridSet GOOGLE_MAPS_COMPATIBLE()
        {
            List<double> resolutions = new List<double>() {
                156543.0339280410,
                78271.51696402048,
                39135.75848201023,
                19567.87924100512,
                9783.939620502561,
                4891.969810251280,
                2445.984905125640,
                1222.992452562820,
                611.4962262814100,
                305.7481131407048,
                152.8740565703525,
                76.43702828517624,
                38.21851414258813,
                19.10925707129406,
                9.554628535647032,
                4.777314267823516,
                2.388657133911758,
                1.194328566955879,
                0.5971642834779395
            };

            return new GridSet(
                "GoogleMapsCompatible",
                "EPSG:3857",
                new Envelope(-20037508.3427892, -20037508.3427892, 20037508.3427892, 20037508.3427892),
                resolutions,
                256,
                256,
                .00028,
                true
            );
        }

        private static IGridSet GOOGLE_CRS84_QUAD()
        {
            throw new NotImplementedException();
        }

        private static IGridSet GLOBAL_CRS84_SCALE()
        {
            throw new NotImplementedException();
        }

        private static IGridSet GLOBAL_CRS84_PIXEL()
        {
            throw new NotImplementedException();
        }

        private static IGridSet SHPERICAL_MERCATOR()
        {
            List<double> resolutions = new List<double>() {
                156543.03390625,
                78271.516953125, 
                39135.7584765625,
                19567.87923828123,
                9783.93961914062,
                4891.96980957031,
                2445.98490478516, 
                1222.99245239258,
                611.496226196289,
                305.7481130981445, 
                152.874056549072, 
                76.4370282745361, 
                38.2185141372681,
                19.109257068634, 
                9.55462853431702, 
                4.77731426715851, 
                2.38865713357925,
                1.19432856678963,
                0.597164283394814,
                0.298582141697407
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
