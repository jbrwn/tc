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
            : this(name, title, cache, provider, gridset, gridset.Envelope, 0, gridset.Grids.Count, provider.getFormats(), 3600, false, false) { }


        public Layer(string name, string title,ICache cache, IProvider provider, GridSet gridset, Envelope bounds, int minZoom, int maxZoom, List<string> formats, int browserCache, bool DisableCache, bool DisableProvider)
        {
            this.Name = name;
            this.Title = title;
            this.cache = cache;
            this.provider = provider;
            this.Gridset = gridset;
            this.bounds = bounds;
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
            this.formats = formats;
            this.browserCache = browserCache;
            this.DisableCache = DisableCache;
            this.DisableProvider = DisableProvider;
        }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public ICache cache { get; set; }

        [DataMember]
        public IProvider provider { get; set; }

        [DataMember(IsRequired = true)]
        public GridSet Gridset { get; set; }

        [DataMember]
        public Envelope bounds { get; set; }

        [DataMember]
        public int MinZoom { get; set; }

        [DataMember]
        public int MaxZoom { get; set; }

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
            if (!this.formats.Contains(format) || !this.provider.getFormats().Contains(format))
            {
                throw new InvalidTileFormatException(string.Format("Invalid tile FORMAT {0}", format)); 
            }

            if (z < this.MinZoom || z > this.MaxZoom)
            {
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", z, this.MinZoom, this.MaxZoom));
            }

            //check if tile within bounds 
            Coord lowCoord = this.Gridset.PointToCoord(new Point(this.bounds.Minx, this.bounds.Miny), z);
            Coord highCoord = this.Gridset.PointToCoord(new Point(this.bounds.Maxx, this.bounds.Maxy), z);

            if (x < lowCoord.X || x > highCoord.X)
            {
                throw new TileOutOfRangeException(string.Format("Column {0} is out of range (min: {1} max: {2})", x, lowCoord.X , highCoord.X));
            }

            if (y < lowCoord.Y || y > highCoord.Y)
            {
                throw new TileOutOfRangeException(string.Format("Row {0} is out of range (min: {1} max: {2})", y, lowCoord.Y, highCoord.Y));
            }

            byte[] img = null;

            // Get tile from cache?
            if (!DisableCache && this.cache != null)
            {
                img = this.cache.Get(z, x, y, format);
            }

            // Get tile from provider ?
            if (img == null && !DisableProvider && this.provider != null)
            {
                if (this.provider is IEnvelopeProvider)
                {
                    Envelope tileEnvelope = this.Gridset.CoordToEnvelope(new Coord(z, x, y));
                    IEnvelopeProvider provider = (IEnvelopeProvider)this.provider;
                    img = provider.render(tileEnvelope, format, this.Gridset.TileWidth, this.Gridset.TileHeight);
                }
                else if (this.provider is IPassThoughProvider)
                {
                    if (this.Gridset.TopOrigin)
                    {
                        y = FlipY(z, y);
                    }

                    IPassThoughProvider provider = (IPassThoughProvider)this.provider;
                    img = provider.render(new Coord(z, x, y), format, this.Gridset.TileWidth, this.Gridset.TileHeight);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected provider type");
                }
                
                // Put tile in cache?
                if (!DisableCache && this.cache != null)
                {
                    cache.Put(z, x, y, format, img);
                }
            }
            return img;
        }

        public int FlipY(int z, int y)
        {
            if (z < this.MinZoom || z > this.MaxZoom)
            {
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", z, this.MinZoom, this.MaxZoom));
            }
            return this.Gridset.GridHeight(z) - y - 1;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //minZoom defaults to 0
            //browserCache defaults to 0
            //DisableCache defaults to false
            //DisableProvider defaults to flase
            if (this.Title == null) { this.Title = ""; }
            if (this.bounds == null) { this.bounds = this.Gridset.Envelope; }
            if (this.MaxZoom == 0) { this.MaxZoom = this.Gridset.Grids.Count; }
            if (this.formats == null) 
            {
                if (this.provider != null) { this.formats = this.provider.getFormats(); }
                else { this.formats = new List<string> {}; }
            }
        }
    }
}
