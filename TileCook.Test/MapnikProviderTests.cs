using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TileCook.Test
{
    [TestClass]
    public class MapnikProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullXml_Throws()
        {
            MapnikProvider m = new MapnikProviderBuilder()
                .SetXmlConfig(null);
        }

        [TestMethod]
        public void Ctor_XmlOnly_DefaultsSetCorrectly()
        {
            MapnikProvider m = new MapnikProviderBuilder()
                .SetXmlConfig(@"..\..\data\empty.xml");

            Assert.AreEqual("png", m.PngOptions);
            Assert.AreEqual("jpeg", m.JpegOptions);
            Assert.AreEqual(0, m.GridLayerIndex);
            Assert.AreEqual(4, m.GridResolution);
            CollectionAssert.AreEquivalent(new List<string>(), m.GridFields);
        }
    }
}
