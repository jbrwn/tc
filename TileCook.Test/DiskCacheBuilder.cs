using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Test
{
    class DiskCacheBuilder
    {
        private string _cacheDirectory;

        public DiskCacheBuilder SetCacheDirectory(string cacheDirectory)
        {
            this._cacheDirectory = cacheDirectory;
            return this;
        }

        public DiskCache Build()
        {
            return new DiskCache(this._cacheDirectory);
        }

        public static implicit operator DiskCache(DiskCacheBuilder obj)
        {
            return obj.Build();
        }



    }
}
