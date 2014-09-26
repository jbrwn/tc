using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Models
{
    public class FilterMap : IMap<ITileFilter,FilterConfig>
    {
        IMap<Envelope, EnvelopeConfig> _envelopeMap;

        public FilterMap(IMap<Envelope, EnvelopeConfig> envelopeMap)
        {
            this._envelopeMap = envelopeMap;
        }

        public ITileFilter Map(FilterConfig obj)
        {
            if (obj != null)
            {
                List<Envelope> extents = null;
                if (obj.Extents != null)
                {
                    foreach (EnvelopeConfig extent in obj.Extents)
                    {
                        extents.Add(this._envelopeMap.Map(extent));
                    }
                }
                return new TileFilter(
                    extents,
                    obj.Formats,
                    obj.ZoomRanges
                );
            }
            return null;
        }
    }
}
