using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TileCook.Test
{
    [TestClass]
    public class LayerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ctor_NullGridSet_throws()
        {
            Layer l = new Layer(
                "test",
                "test layer", 
                null, 
                new DiskCache(@".\"),
                new TestProvider()
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ctor_NullName_throws()
        {
            Layer l = new Layer(
                null,
                "test layer",
                new GridSet( "Spherical Mercator", "EPSG:900913", new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789), 18, 256, 1, false),
                new DiskCache(@".\"),
                new TestProvider()
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ctor_NullProviderNullFormats_throws()
        {
            Layer l = new Layer(
                "test",
                "test layer",
                new GridSet("Spherical Mercator", "EPSG:900913", new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789), 18, 256, 1, false),
                new DiskCache(@".\"),
                null,
                new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789),
                0,
                0,
                null,
                0,
                false,
                false
            );
        }

        [TestMethod]
        public void ctor_GridSetOnly_DefaultsSetCorrectly()
        {
            Layer l = new Layer(
                "test",
                "test layer",
                new GridSet("Spherical Mercator", "EPSG:900913", new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789), 18, 256, 1, false),
                new DiskCache(@".\"),
                new TestProvider(),
                null,
                0,
                0,
                null,
                0,
                false,
                false
            );

            Assert.IsTrue(l.Bounds.Equals(new Envelope(20037508.342789, 20037508.342789, -20037508.342789, -20037508.342789)));
            Assert.AreEqual(l.MaxZoom, 18);
            Assert.IsNotNull(l.Formats);
            CollectionAssert.AreEquivalent((List<string>)l.Formats, l.Provider.GetFormats());
        }


    }
}
