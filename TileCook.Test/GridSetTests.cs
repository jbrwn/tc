using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileCook;

namespace TileCook.Test
{
    [TestClass]
    public class GridSetTests
    {
        private GridSet SphericalMercatorGrid()
        {
            Envelope env = new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789);
            int tileSize = 256;
            int levels = 18;

            return new GridSet(
                "test",
                "EPSG:900913",
                env,
                levels,
                tileSize,
                1,
                false
            );
        }
        
        [TestMethod]
        public void Ctor_SphericalMercator_GridsCreated()
        {
            GridSet g = SphericalMercatorGrid();
            Assert.AreEqual(18, g.Grids.Count);
        }

        [TestMethod]
        public void CoordToEnvelope_SphericalMercator_ReturnsEnvelope()
        {
            GridSet g = SphericalMercatorGrid();
            Envelope env = g.Envelope;

            // 0/0/0 coord should match gridset envelope
            Assert.IsTrue(env.Equals(g.CoordToEnvelope(new Coord(0, 0, 0))));
        }

        [TestMethod]
        public void GridWidth_SphericalMercator_ReturnsWidth()
        {
            GridSet g = SphericalMercatorGrid();
            Assert.AreEqual(4, g.GridWidth(2));
        }

        [TestMethod]
        public void GridHeight_SphericalMercator_ReturnsHeight()
        {
            GridSet g = SphericalMercatorGrid();
            Assert.AreEqual(4, g.GridHeight(2));
        }
    }
}
