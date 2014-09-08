using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TileCook.Test
{
    [TestClass]
    public class MapnikVectorProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullXml_Throws()
        {

        }

        [TestMethod]
        public void Ctor_XmlTileSourceOnly_DefaultsSetCorrectly()
        {

        }
    }
}
