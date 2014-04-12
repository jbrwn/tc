using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class Layer
    {

        private Layer()
        {
        }

        public Layer(string name, ICache cache, IProvider provider, GridSet gridset)
            : this(name, cache, provider, gridset, gridset.envelope, 0, gridset.grids.Count, new List<string> { "png", "jpg" }, 3600) { }


        public Layer(string name,ICache cache, IProvider provider, GridSet gridset, Envelope bounds, int minZoom, int maxZoom, List<string> formats, int browserCache)
        {
            this.name = name;
            this.cache = cache;
            this.provider = provider;
            this.gridset = gridset;
            this.bounds = bounds;
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
            this.formats = formats;
            this.browserCache = browserCache;
        }

        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember(IsRequired = true)]
        public ICache cache { get; set; }

        [DataMember(IsRequired = true)]
        public IProvider provider { get; set; }

        [DataMember(IsRequired = true)]
        public GridSet gridset { get; set; }

        [DataMember]
        public Envelope bounds { get; set; }

        [DataMember]
        public int minZoom { get; set; }

        [DataMember]
        public int maxZoom { get; set; }

        [DataMember]
        public List<string> formats { get; set; }

        [DataMember]
        public int browserCache { get; set; }
 
        public byte[] getTile(int z, int x, int y, string format)
        {
            //check if format supported
            if (!formats.Contains(format))
            {
                throw new InvalidTileFormatException("Invalid tile format"); 
            }

            Envelope tileEnvelope = this.gridset.tileToEnvelope(z, x, y);
            
            //check if tile within bounds 
            Tile lowTile = this.gridset.PointToTile(new Point(this.bounds.minx, this.bounds.miny), z);
            Tile highTile = this.gridset.PointToTile(new Point(this.bounds.maxx, this.bounds.maxy), z);
            
            if (x < lowTile.x || x > highTile.x)
            {
                throw new TileOutOfRangeException("Tile out of range");
            }

            if (y < lowTile.y || y > highTile.y)
            {
                throw new TileOutOfRangeException("Tile out of range");
            }

            byte[] img = null;
            img = this.cache.get(z, x, y, format);
            
            if (img == null)
            {
                img = this.provider.render(tileEnvelope, format, this.gridset.tileWidth, this.gridset.tileHeight);
                cache.put(z, x, y, format, img);
            }
            return img;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //minZoom defaults to 0
            //browserCache defaults to 0
            if (this.bounds == null) { this.bounds = this.gridset.envelope; }
            if (this.maxZoom == 0) { this.maxZoom = this.gridset.grids.Count; }
            if (this.formats == null) { this.formats = this.provider.getFormats(); }
        }
    }
}
