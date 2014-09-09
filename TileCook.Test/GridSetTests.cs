using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileCook;
using Moq;

namespace TileCook.Test
{
    [TestClass]
    public class GridSetTests
    {
        [TestMethod]
        public void Ctor_SphericalMercator_GridsCreated()
        {
            GridSet g = new GridSetBuilder();
            Assert.AreEqual(18, g.Grids.Count);
        }

        [TestMethod]
        public void CoordToEnvelope_SphericalMercator_ReturnsEnvelope()
        {
            GridSet g = new GridSetBuilder();
            Envelope env = g.Envelope;

            // 0/0/0 coord should match gridset envelope
            Assert.IsTrue(env.Equals(g.CoordToEnvelope(new Coord(0, 0, 0))));
        }

        [TestMethod]
        public void GridWidth_SphericalMercator_ReturnsWidth()
        {
            GridSet g = new GridSetBuilder();
            Assert.AreEqual(4, g.GridWidth(2));
        }

        [TestMethod]
        public void GridHeight_SphericalMercator_ReturnsHeight()
        {
            GridSet g = new GridSetBuilder();
            Assert.AreEqual(4, g.GridHeight(2));
        }
    }
}
