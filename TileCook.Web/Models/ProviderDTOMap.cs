using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class ProviderDTOMap : IMap<IProvider, ProviderDTO>
    {
        public IProvider Map(ProviderDTO obj)
        {
            if (obj.Type != null)
            {
                // use factory class?
                switch (obj.Type.ToLower())
                {
                    case "mapnik":
                        return new MapnikProvider(obj.XmlConfig);
                    case "mapnikvector":
                        LayerDTOMap tileSourceMap = new LayerDTOMap();
                        Layer tileSource = tileSourceMap.Map(obj.TileSource);
                        return new MapnikVectorTileProvider(obj.XmlConfig, tileSource);
                    case "proxy":
                        return new ProxyProvider(obj.UrlTemplate);
                    case "test":
                        return new TestProvider();
                    case "wms":
                        return new WMSProvider(obj.Url, obj.Version, obj.Layers, obj.CRS, obj.Styles, obj.Format, obj.SLD);
                }
            }
            return null;
        }

        public ProviderDTO Map(IProvider obj)
        {
            throw new NotImplementedException();
        }
    }
}