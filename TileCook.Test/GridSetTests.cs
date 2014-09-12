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
        public void CoordToEnvelope_SimpleGridset_ReturnsEnvelope()
        {
            GridSet g = new GridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 100, 100),
                new List<double>() { 10, 5, 2.5},
                10,
                10
            );

            Envelope expected = new Envelope(0, 0, 100, 100);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(0, 0, 0))));

            expected = new Envelope(0, 0, 50, 50);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(1, 0, 0))));

            expected = new Envelope(50, 50, 75, 75);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(2, 2, 2))));
        }

        [TestMethod]
        public void CoordToEnvelope_SimpleGridsetTopOrgin_ReturnsEnvelope()
        {
            GridSet g = new GridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 100, 100),
                new List<double>() { 10, 5, 2.5 },
                10,
                10,
                0,
                true
            );

            Envelope expected = new Envelope(0, 0, 100, 100);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(0, 0, 0))));

            expected = new Envelope(0, 0, 50, 50);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(1, 0, 1))));

            expected = new Envelope(75, 75, 100, 100);
            Assert.IsTrue(expected.Equals(g.CoordToEnvelope(new Coord(2, 3, 0))));
        }

        [TestMethod]
        public void GridHeight_SimpleGridset_ReturnsHeight()
        {
            GridSet g = new GridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 100, 100),
                new List<double>() { 10, 5, 2.5 },
                10,
                10
            );

            Assert.AreEqual(g.GridHeight(2), 4);
        }

        [TestMethod]
        public void GridWidth_SimpleGridset_ReturnsWidth()
        {
            GridSet g = new GridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 100, 100),
                new List<double>() { 10, 5, 2.5 },
                10,
                10
            );

            Assert.AreEqual(g.GridWidth(2), 4);
        }


    }
}
