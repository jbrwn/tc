using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TileCook.Test
{
    [TestClass]
    public class TileFilterTests
    {
        [TestMethod]
        public void IsValid_NullCollections_ReturnsTrue()
        {
            TileFilter f = new TileFilter(null, null, null);
            Assert.IsTrue(f.IsValid(
                0,
                new Envelope(0, 0, 0, 0),
                "test format")
            );
        }

        [TestMethod]
        public void IsEnvelopeValid_DisjointExtent_ReturnsFalse()
        {
            TileFilter f = new TileFilter(
                new List<Envelope>() { new Envelope(0,0,10,10) },
                null,
                null
            );

            Assert.IsFalse(
                f.IsEnvelopeValid(
                    new Envelope(12, 12, 44, 44)
                )
            );
        }

        [TestMethod]
        public void IsEnvelopeValid_IntersectingExtent_ReturnsTrue()
        {
            TileFilter f = new TileFilter(
                new List<Envelope>() { new Envelope(0, 0, 10, 10), new Envelope(11,11,50,50) },
                null,
                null
            );

            Assert.IsTrue(
                f.IsEnvelopeValid(
                    new Envelope(12, 12, 44, 44)
                )
            );
        }

        [TestMethod]
        public void IsFormatValid_ValidFormat_ReturnsTrue()
        {
            TileFilter f = new TileFilter(
                null,
                new List<string>() { "jpg" },
                null
            );

            Assert.IsTrue(f.IsFormatValid("JPG"));
        }

        [TestMethod]
        public void IsFormatValid_InvalidFormat_ReturnsFalse()
        {
            TileFilter f = new TileFilter(
                null,
                new List<string>() { "png" },
                null
            );

            Assert.IsFalse(f.IsFormatValid("JPG"));
        }

        [TestMethod]
        public void IsZValid_ValidZ_ReturnsTrue()
        {
            TileFilter f = new TileFilter(
                null,
                null,
                new List<Tuple<int,int>>() { new Tuple<int,int>(0,5), new Tuple<int,int>(9,15)}
            );

            Assert.IsTrue(f.IsZValid(11));
        }

        [TestMethod]
        public void IsZValid_InvalidZ_ReturnsFalse()
        {
            TileFilter f = new TileFilter(
                null,
                null,
                new List<Tuple<int, int>>() { new Tuple<int, int>(0, 5), new Tuple<int, int>(9, 15) }
            );

            Assert.IsFalse(f.IsZValid(6));
        }
    }
}
