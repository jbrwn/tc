using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Models
{
    public class GridSetMap : IMap<IGridSet, GridSetConfig>
    {
        private readonly IMap<Envelope, EnvelopeConfig> _EnvelopeMap;

        public GridSetMap(IMap<Envelope, EnvelopeConfig> EnvelopeMap)
        {
            this._EnvelopeMap = EnvelopeMap;
        }

        public IGridSet Map(GridSetConfig obj)
        {
            if (obj != null)
            {
                // Check if name is well known gridset
                WellKnownGridSet wkgs;
                if (Enum.TryParse<WellKnownGridSet>(obj.Name, out wkgs))
                    return GridSetFactory.CreateGridSet(wkgs);

                return GridSetFactory.CreateGridSet(obj.Name, obj.SRS, this._EnvelopeMap.Map(obj.Envelope), obj.TileSize, obj.Levels, obj.Step, obj.PixelSize, obj.TopOrigin);
            }
            return null;
        }
    }
}