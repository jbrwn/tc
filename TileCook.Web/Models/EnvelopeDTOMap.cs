using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Web.Models
{
    public class EnvelopeDTOMap : IMap<Envelope, EnvelopeDTO>
    {
        public Envelope Map(EnvelopeDTO obj)
        {
            if (obj != null)
            {
                return new Envelope(
                    obj.Minx,
                    obj.Miny,
                    obj.Maxx,
                    obj.Maxy
                );
            }
            return null;
        }

        public EnvelopeDTO Map(Envelope obj)
        {
            throw new NotImplementedException();
        }
    }
}