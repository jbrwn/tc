using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace TileCook.Test
{
    [TestClass]
    public class LayerTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullGridSet_throws()
        {
            Layer l = new LayerBuilder()
                .Initialize()
                .SetGridSet(null);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullName_throws()
        {
            Layer l = new LayerBuilder()
                .Initialize()
                .SetName(null);
        }

        [TestMethod]
        public void Ctor_BareBonesArgs_DefaultsSetCorrectly()
        {
            Layer l = new LayerBuilder()
                .Initialize();

            Assert.IsTrue(l.Bounds.Equals(new Envelope(0, 0, 0, 0)));
            Assert.AreEqual(l.MaxZoom, 0);
            Assert.IsNotNull(l.Formats);
        }

        [TestMethod]
        [ExpectedException(typeof(TileOutOfRangeException))]
        public void GetTile_ZOutOfRange_Throws()
        {
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(p => p.GetFormats()).Returns(new List<string>() { "png" });

            Layer l = new LayerBuilder()
                .Initialize()
                .SetProvider(mockProvider.Object);

            l.GetTile(1, 0, 0, "png");
        }
    }
}
