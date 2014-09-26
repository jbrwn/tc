using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public interface ITileFilter
    {
        bool IsEnvelopeValid(Envelope env);
        bool IsFormatValid(string format);
        bool IsZValid(int Z);
        bool IsValid(int Z, Envelope env, string format);
    }
}
