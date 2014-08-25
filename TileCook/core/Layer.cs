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

        public Layer(string name, string title, ICache cache, IProvider provider, GridSet gridset)
            : this(name, title, cache, provider, gridset, gridset.envelope, 0, gridset.grids.Count, provider.getFormats(), 3600, false, false) { }


        public Layer(string name, string title,ICache cache, IProvider provider, GridSet gridset, Envelope bounds, int minZoom, int maxZoom, List<string> formats, int browserCache, bool DisableCache, bool DisableProvider)
        {
            this.name = name;
            this.Title = title;
            this.cache = cache;
            this.provider = provider;
            this.gridset = gridset;
            this.bounds = bounds;
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
            this.formats = formats;
            this.browserCache = browserCache;
            this.DisableCache = DisableCache;
            this.DisableProvider = DisableProvider;
        }

        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public ICache cache { get; set; }

        [DataMember]
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

        [DataMember]
        public bool DisableCache { get; set; }

        [DataMember]
        public bool DisableProvider { get; set; }
 
        public byte[] getTile(int z, int x, int y, string format)
        {
            //check if format supported
            if (!formats.Contains(format))
            {
                throw new InvalidTileFormatException(string.Format("Invalid tile FORMAT {0}", format)); 
            }

            if (z < minZoom || z > maxZoom)
            {
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", z, this.minZoom, this.maxZoom));
            }

            //check if tile within bounds 
            Coord lowCoord = this.gridset.PointToCoord(new Point(this.bounds.minx, this.bounds.miny), z);
            Coord highCoord = this.gridset.PointToCoord(new Point(this.bounds.maxx, this.bounds.maxy), z);

            if (x < lowCoord.x || x > highCoord.x)
            {
                throw new TileOutOfRangeException(string.Format("Column {0} is out of range (min: {1} max: {2})", x, lowCoord.x , highCoord.x));
            }

            if (y < lowCoord.y || y > highCoord.y)
            {
                throw new TileOutOfRangeException(string.Format("Row {0} is out of range (min: {1} max: {2})", y, lowCoord.y, highCoord.y));
            }

            byte[] img = null;

            // Get tile from cache?
            if (!DisableCache && this.cache != null)
            {
                img = this.cache.get(z, x, y, format);
            }

            // Get tile from provider ?
            if (img == null && !DisableProvider && this.provider != null)
            {
                if (this.provider is IEnvelopeProvider)
                {
                    Envelope tileEnvelope = this.gridset.CoordToEnvelope(new Coord(z, x, y));
                    IEnvelopeProvider provider = (IEnvelopeProvider)this.provider;
                    img = provider.render(tileEnvelope, format, gridset.tileWidth, gridset.tileHeight);
                }
                else if (this.provider is IPassThoughProvider)
                {
                    if (gridset.topOrigin)
                    {
                        y = FlipY(z, y);
                    }

                    IPassThoughProvider provider = (IPassThoughProvider)this.provider;
                    img = provider.render(new Coord(z, x, y), format, gridset.tileWidth, gridset.tileHeight);
                }
                else
                {
                    throw new InvalidOperationException();
                }
                
                // Put tile in cache?
                if (!DisableCache && this.cache != null)
                {
                    cache.put(z, x, y, format, img);
                }
            }
            return img;
        }

        public int FlipY(int z, int y)
        {
            if (z < this.minZoom || z > this.maxZoom)
            {
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", z, this.minZoom, this.maxZoom));
            }
            return this.gridset.gridHeight(z) - y - 1;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //minZoom defaults to 0
            //browserCache defaults to 0
            //DisableCache defaults to false
            //DisableProvider defaults to flase
            if (this.Title == null) { this.Title = ""; }
            if (this.bounds == null) { this.bounds = this.gridset.envelope; }
            if (this.maxZoom == 0) { this.maxZoom = this.gridset.grids.Count; }
            if (this.formats == null) 
            {
                if (this.provider != null) { this.formats = this.provider.getFormats(); }
                else { this.formats = new List<string> {}; }
            }
        }
    }
}
