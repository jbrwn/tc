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
            : this(name, title, cache, provider, gridset, gridset.Envelope, 0, gridset.Grids.Count, provider.GetFormats(), 3600, false, false) { }


        public Layer(string name, string title,ICache cache, IProvider provider, GridSet gridset, Envelope bounds, int minZoom, int maxZoom, List<string> formats, int browserCache, bool DisableCache, bool DisableProvider)
        {
            this.Name = name;
            this.Title = title;
            this.Cache = cache;
            this.Provider = provider;
            this.Gridset = gridset;
            this.Bounds = bounds;
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
            this.Formats = formats;
            this.BrowserCache = browserCache;
            this.DisableCache = DisableCache;
            this.DisableProvider = DisableProvider;
        }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public ICache Cache { get; set; }

        [DataMember]
        public IProvider Provider { get; set; }

        [DataMember(IsRequired = true)]
        public GridSet Gridset { get; set; }

        [DataMember]
        public Envelope Bounds { get; set; }

        [DataMember]
        public int MinZoom { get; set; }

        [DataMember]
        public int MaxZoom { get; set; }

        [DataMember]
        public List<string> Formats { get; set; }

        [DataMember]
        public int BrowserCache { get; set; }

        [DataMember]
        public bool DisableCache { get; set; }

        [DataMember]
        public bool DisableProvider { get; set; }
 
        public byte[] GetTile(int z, int x, int y, string format)
        {
            //check if format supported
            if (!this.Formats.Contains(format) || !this.Provider.GetFormats().Contains(format))
            {
                throw new InvalidTileFormatException(string.Format("Invalid tile FORMAT {0}", format)); 
            }

            if (z < this.MinZoom || z > this.MaxZoom)
            {
                throw new TileOutOfRangeException(string.Format("Zoom level {0} is out of range (min: {1} max: {2})", z, this.MinZoom, this.MaxZoom));
            }

            //check if tile within bounds 
            Coord lowCoord = this.Gridset.PointToCoord(new Point(this.Bounds.Minx, this.Bounds.Miny), z);
            Coord highCoord = this.Gridset.PointToCoord(new Point(this.Bounds.Maxx, this.Bounds.Maxy), z);

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
            if (!DisableCache && this.Cache != null)
            {
                img = this.Cache.Get(z, x, y, format);
            }

            // Get tile from provider ?
            if (img == null && !DisableProvider && this.Provider != null)
            {
                if (this.Provider is IEnvelopeProvider)
                {
                    Envelope tileEnvelope = this.Gridset.CoordToEnvelope(new Coord(z, x, y));
                    IEnvelopeProvider provider = (IEnvelopeProvider)this.Provider;
                    img = provider.Render(tileEnvelope, format, this.Gridset.TileWidth, this.Gridset.TileHeight);
                }
                else if (this.Provider is IPassThoughProvider)
                {
                    if (this.Gridset.TopOrigin)
                    {
                        y = FlipY(z, y);
                    }

                    IPassThoughProvider provider = (IPassThoughProvider)this.Provider;
                    img = provider.Render(new Coord(z, x, y), format, this.Gridset.TileWidth, this.Gridset.TileHeight);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected provider type");
                }
                
                // Put tile in cache?
                if (!DisableCache && this.Cache != null)
                {
                    this.Cache.Put(z, x, y, format, img);
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
            if (this.Bounds == null) { this.Bounds = this.Gridset.Envelope; }
            if (this.MaxZoom == 0) { this.MaxZoom = this.Gridset.Grids.Count; }
            if (this.Formats == null) 
            {
                if (this.Provider != null) { this.Formats = this.Provider.GetFormats(); }
                else { this.Formats = new List<string> {}; }
            }
        }
    }
}
