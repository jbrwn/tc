using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public interface IGridSet
    {
        string Name { get; }
        string SRS { get; }
        double MetersPerUnit { get; }
        Envelope Envelope { get; }
        List<Grid> Grids { get; }
        int TileWidth { get; }
        int TileHeight { get; }
        bool TopOrigin { get; }

        double Resolution(int z);
        int GridWidth(int z);
        int GridHeight(int z);
        Envelope CoordToEnvelope(Coord coord);
        Coord PointToCoord(Point p, int z);
    }
}
