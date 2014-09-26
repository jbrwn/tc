using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Models
{
    public class ProviderMap : IMap<IProvider, ProviderConfig>
    {
        private readonly IMap<Layer, LayerConfig> _LayerMap;
        private readonly IPathResolver _pathResolver;

        public ProviderMap(IMap<Layer, LayerConfig> LayerMap, IPathResolver pathResolver)
        {
            this._pathResolver = pathResolver;
            this._LayerMap = LayerMap;
        }

        public IProvider Map(ProviderConfig obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Type))
            {
                // use factory class?
                switch (obj.Type.ToLower())
                {
                    case "mapnik":
                        return new MapnikProvider(this._pathResolver.ResolvePath(obj.XmlConfig));
                    case "mapnikvector":
                        return new MapnikVectorTileProvider(
                            this._pathResolver.ResolvePath(obj.XmlConfig),
                            this._LayerMap.Map(obj.TileSource)
                        );
                    case "proxy":
                        return new ProxyProvider(obj.UrlTemplate, obj.Format);
                    case "test":
                        return new TestProvider();
                    case "wms":
                        return new WMSProvider(obj.Url, obj.Version, obj.Layers, obj.CRS, obj.Styles, obj.Format, obj.SLD);
                }
            }
            return null;
        }
    }
}