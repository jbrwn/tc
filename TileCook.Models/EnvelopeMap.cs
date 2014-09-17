using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileCook.Models
{
    public class EnvelopeMap : IMap<Envelope, EnvelopeConfig>
    {
        public Envelope Map(EnvelopeConfig obj)
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
    }
}