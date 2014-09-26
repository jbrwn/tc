using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
        public struct ZoomRange
        {
            private readonly int _minZoom;
            private readonly int _maxZoom;

            public ZoomRange(int minZoom, int maxZoom)
            {
                this._minZoom = minZoom;
                this._maxZoom = maxZoom;
            }
            public int MinZoom { get { return this._minZoom; } }
            public int MaxZoom { get { return this._maxZoom; } }
        }

    public class TileFilter : ITileFilter
    {
        public readonly IEnumerable<Envelope> _extents;
        public readonly IEnumerable<string> _formats;
        public readonly IEnumerable<ZoomRange> _zLevels;

        public TileFilter(IEnumerable<Envelope> extents, IEnumerable<string> formats, IEnumerable<ZoomRange> zLevels)
        {
            this._extents = extents;
            this._formats = formats;
            this._zLevels = zLevels;
        }

        public bool IsEnvelopeValid(Envelope other)
        {
            if (this._extents == null)
                return true;

            foreach (Envelope extent in this._extents)
            {
                if (extent.intersects(other))
                    return true;
            }

            return false;
           
        }

        public bool IsFormatValid(string other)
        {
            if (this._formats == null)
                return true;
        
            foreach(string format in this._formats)
            {
                if (format.ToLower() == other.ToLower())
                    return true;
            }

            return false;
        }


        public bool IsZValid(int Z)
        {
            if (this._zLevels == null)
                return true;
        
            foreach(ZoomRange range in this._zLevels)
            {
                if (Z >= range.MinZoom && Z <= range.MaxZoom)
                    return true;
            }

            return false;
        }

        public bool IsValid(int Z, Envelope env, string format)
        {
            return IsEnvelopeValid(env) && IsFormatValid(format) && IsZValid(Z);
        }
    }
}
