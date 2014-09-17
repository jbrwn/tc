using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Models
{
    public class CacheMap : IMap<ICache, CacheConfig>
    {
        public readonly IPathResolver _pathResolver;

        public CacheMap(IPathResolver pathResolver)
        {
            this._pathResolver = pathResolver;
        }

        public ICache Map(CacheConfig obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Type))
            {
                switch (obj.Type.ToLower())
                {
                    case "disk":
                        return new DiskCache(this._pathResolver.ResolvePath(obj.CacheDirectory));
                    case "mbtiles":
                        return new MBTilesCache(this._pathResolver.ResolvePath(obj.Database), obj.Format);
                }
            }
            return null;
        }
    }
}