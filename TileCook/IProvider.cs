using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public interface IProvider
    {
        byte[] render(Envelope envelope, string format, int tileWidth, int tileHeight);
        List<string> getFormats();
    }
}
