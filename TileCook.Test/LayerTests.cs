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
                .SetName("test")
                .SetProvider(new Mock<IProvider>().Object)
                .SetGridSet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullName_throws()
        {
            Layer l = new LayerBuilder()
                .SetName(null)
                .SetProvider(new Mock<IProvider>().Object)
                .SetGridSet(new Mock<IGridSet>().Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullProviderNullFormats_throws()
        {
            Layer l = new LayerBuilder()
                .SetName("test")
                .SetProvider(null)
                .SetGridSet(new Mock<IGridSet>().Object);
        }

        [TestMethod]
        public void Ctor_MinimalArgs_DefaultsSetCorrectly()
        {
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(m=> m.GetFormats()).Returns(new List<string>());

            var mockGridSet = new Mock<IGridSet>();
            mockGridSet.Setup(m => m.Grids.Count).Returns(18);
            mockGridSet.Setup(m => m.Envelope).Returns(new Envelope(0, 0, 1, 1));

            Layer l = new LayerBuilder()
                .SetName("test")
                .SetProvider(mockProvider.Object)
                .SetGridSet(mockGridSet.Object);

            Assert.IsTrue(l.Bounds.Equals(new Envelope(0, 0, 1, 1)));
            Assert.AreEqual(l.MaxZoom, 18);
            Assert.IsNotNull(l.Formats);
            CollectionAssert.AreEquivalent(l.Formats, l.Provider.GetFormats());
        }


    }
}
