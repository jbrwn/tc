using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class GridSetDTOMap : IMap<IGridSet, GridSetDTO>
    {
        public IGridSet Map(GridSetDTO obj)
        {
            if (obj != null)
            {
                // Check if name is well known gridset
                WellKnownGridSet wkgs;
                if (Enum.TryParse<WellKnownGridSet>(obj.Name, out wkgs))
                    return GridSetFactory.CreateGridSet(wkgs);

                // get envelope
                EnvelopeDTOMap envMap = new EnvelopeDTOMap();
                Envelope env = envMap.Map(obj.Envelope);

                return GridSetFactory.CreateGridSet(obj.Name, obj.SRS, env, obj.TileSize, obj.Levels, obj.Step, obj.PixelSize, obj.TopOrigin);
            }
            return null;
        }

        public GridSetDTO Map(IGridSet obj)
        {
            throw new NotImplementedException();
        }
    }
}