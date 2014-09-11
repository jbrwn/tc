using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class CacheDTOMap : IMap<ICache, CacheDTO>
    {
        public ICache Map(CacheDTO obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Type))
            {
                // use dedicated factory class?
                switch (obj.Type.ToLower())
                {
                    case "disk":
                        return new DiskCache(obj.CacheDirectory);
                    case "mbtiles":
                        return new MBTilesCache(obj.Database, obj.Format);
                }
            }
            return null;
        }

        public CacheDTO Map(ICache obj)
        {
            throw new NotImplementedException();
        }
    }
}