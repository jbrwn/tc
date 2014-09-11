using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileCook;

namespace TileCook.Test
{
    [TestClass]
    public class DiskCacheTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullDirectory_Throws()
        {
            DiskCache dc = new DiskCacheBuilder()
                .SetCacheDirectory(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_EmptyDirectory_Throws()
        {
            DiskCache dc = new DiskCacheBuilder()
                .SetCacheDirectory("");
        }
    }
}
