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
            return new Envelope(
                obj.Minx,
                obj.Miny,
                obj.Maxx,
                obj.Maxy
            );
        }

        public EnvelopeDTO Map(Envelope obj)
        {
            throw new NotImplementedException();
        }
    }
}