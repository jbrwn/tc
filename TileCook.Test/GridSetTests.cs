using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace TileCook.Test
{
    [TestClass]
    public class GridSetTests
    {

        [TestMethod]
        public void CoordToEnvelope_SimpleGrid_ReturnsEnvelope()
        {
            GridSet g = new GridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 100, 100),
                new List<double>() { 10, 5, 1},
                10,
                10
            );

            // 0/0/0 coord should match gridset envelope
            Envelope expected = new Envelope(0, 0, 100, 100);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(0, 0, 0))));

            expected = new Envelope(0, 0, 50, 50);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(1, 1, 1))));
        }

        //[TestMethod]
        //public void GridWidth_SphericalMercator_ReturnsWidth()
        //{
        //    GridSet g = new GridSetBuilder()
        //        .SphericalMercator();
        //    Assert.AreEqual(4, g.GridWidth(2));
        //}

        //[TestMethod]
        //public void GridHeight_SphericalMercator_ReturnsHeight()
        //{
        //    GridSet g = new GridSetBuilder()
        //        .SphericalMercator();
        //    Assert.AreEqual(4, g.GridHeight(2));
        //}

        //[TestMethod]
        //public void Ctor_0MetersPerUnit_MetersPerUnitSetTo1()
        //{
        //    GridSet g = new GridSetBuilder()
        //        .SphericalMercator()
        //        .SetMetersPerUnit(0);
        //    Assert.AreEqual(1.0, g.MetersPerUnit);
        //}

    }
}
