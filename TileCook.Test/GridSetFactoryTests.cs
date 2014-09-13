using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TileCook.Test
{
    [TestClass]
    public class GridSetFactoryTests
    {
        [TestMethod]
        public void Build_SimpleGridWithDefaults_ResolutionsCreated()
        {
            IGridSet gs = GridSetFactory.CreateGridSet(
                "test",
                "epsg:test",
                new Envelope(0, 0, 512, 512),
                256,
                3,
                2
            );

            List<double> r = gs.Resolutions as List<double>;

            Assert.AreEqual(r.Count, 3);
            Assert.AreEqual(r[0], 2);
            Assert.AreEqual(r[1], 1);
            Assert.AreEqual(r[2], .5);
        }
    }
}
