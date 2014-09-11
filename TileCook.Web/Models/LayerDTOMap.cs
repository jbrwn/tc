using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class LayerDTOMap : IMap<Layer,LayerDTO>
    {
        public Layer Map(LayerDTO obj)
        {
            // Gridset
            GridSetDTOMap gridSetMap = new GridSetDTOMap();
            IGridSet gridset = gridSetMap.Map(obj.Gridset);

            // Cache 
            CacheDTOMap cacheMap = new CacheDTOMap();
            ICache cache = cacheMap.Map(obj.Cache);

            // Provider 
            ProviderDTOMap providerMap = new ProviderDTOMap();
            IProvider provider = providerMap.Map(obj.Provider);

            // Bounds 
            EnvelopeDTOMap envMap = new EnvelopeDTOMap();
            Envelope bounds = envMap.Map(obj.Bounds);

            return new Layer(
                obj.Name,
                obj.Title,
                gridset,
                cache,
                provider,
                bounds,
                obj.MinZoom,
                obj.MaxZoom,
                obj.Formats,
                obj.BrowserCache,
                obj.DisableCache,
                obj.DisableProvider
            );

        }

        public LayerDTO Map(Layer obj)
        {
            throw new NotImplementedException();
        }
    }
}