using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class GridSetDTOMap : IMap<GridSet, GridSetDTO>
    {
        public GridSet Map(GridSetDTO obj)
        {
            if (obj != null)
            {
                // Special case - well known gridset is provided
                if (
                    obj.Name != null &&
                    obj.SRS == null &&
                    obj.Envelope == null &&
                    obj.TileSize == 0 &&
                    obj.ZoomLevels == 0 &&
                    obj.MetersPerUnit == 0 &&
                    obj.TopOrigin == false
                )
                {
                    return new GridSet(obj.Name);
                }

                // get envelope
                EnvelopeDTOMap envMap = new EnvelopeDTOMap();
                Envelope env = envMap.Map(obj.Envelope);

                // return new gridset
                return new GridSet(
                        obj.Name,
                        obj.SRS,
                        env,
                        obj.ZoomLevels,
                        obj.TileSize,
                        obj.MetersPerUnit,
                        obj.TopOrigin
                );
            }
            return null;
        }

        public GridSetDTO Map(GridSet obj)
        {
            throw new NotImplementedException();
        }
    }
}