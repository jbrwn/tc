using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TileCook;

namespace TileCook.Web.Models
{
    public static class DTOMapper
    {
        public static Layer Map(LayerDTO layerDTO)
        {


            //Layer layer = new Layer();
            return null;
        }

        private GridSet Map(GridSetDTO gridSetDTO)
        {
            // Special case - well known gridset is provided
            if (
                gridSetDTO.Name != null &&
                gridSetDTO.SRS == null &&
                gridSetDTO.Envelope == null &&
                gridSetDTO.TileSize == 0 &&
                gridSetDTO.ZoomLevels == 0 &&
                gridSetDTO.MetersPerUnit == 0 &&
                gridSetDTO.TopOrigin == false
            )
            {
                return new GridSet(gridSetDTO.Name);
            }

            // Validate paramaters
            if (
                gridSetDTO.Name == null ||
                gridSetDTO.SRS == null ||
                gridSetDTO.Envelope == null ||
                gridSetDTO.TileSize == 0 ||
                gridSetDTO.ZoomLevels == 0 ||
                gridSetDTO.MetersPerUnit == 0 
            )
            {
                throw new Exception();
            }

            // get envelope
            Envelope env = this.Map(gridSetDTO.Envelope);

            // return new gridset
            return new GridSet(
                    gridSetDTO.Name,
                    gridSetDTO.SRS,
                    env,
                    gridSetDTO.ZoomLevels,
                    gridSetDTO.TileSize,
                    gridSetDTO.MetersPerUnit,
                    gridSetDTO.TopOrigin
            );
        }

        private Envelope Map(EnvelopeDTO envelopeDTO)
        {
            return new Envelope(
                envelopeDTO.Minx,
                envelopeDTO.Miny,
                envelopeDTO.Maxx,
                envelopeDTO.Maxy
            );
        }

        private ICache Map(CacheDTO cacheDTO)
        {
            switch (cacheDTO.Type.ToLower())
            {
                case "disk":
                    //validate disk properties
                    if (string.IsNullOrEmpty(cacheDTO.CacheDirectory))
                    {
                        throw new ArgumentNullException();
                    }
                    return new DiskCache(cacheDTO.CacheDirectory);
                case "mbtiles":
                    if (string.IsNullOrEmpty(cacheDTO.Database)
                         || string.IsNullOrEmpty(cacheDTO.Format))
                    {
                        throw new ArgumentNullException();
                    }
                    return new MBTilesCache(cacheDTO.Database, cacheDTO.Format);
                default:
                    throw new NotImplementedException();
            }
        }

        private IProvider Map(ProviderDTO providerDTO)
        {
            throw new NotImplementedException();
        }

    }
}