using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models.Config
{
    public class VectorTileProviderMap : IMap<IVectorTileProvider, ProviderConfig>
    {
        private readonly IPathResolver _pathResolver;

        public VectorTileProviderMap(IPathResolver pathResolver)
        {
            this._pathResolver = pathResolver;
        }

        public IVectorTileProvider Map(ProviderConfig obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Type))
            {
                // use factory class?
                switch (obj.Type.ToLower())
                {
                    case "mapnik":
                        return new MapnikProvider(this._pathResolver.ResolvePath(obj.XmlConfig));
                    case "proxy":
                        return new ProxyProvider(obj.UrlTemplate);
                }
            }
            return null;
        }
    }
}