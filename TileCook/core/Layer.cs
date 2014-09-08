using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TileCook
{
    public class Layer
    {
        private string _name;
        private string _title;
        private ICache _cache;
        private IProvider _provider;
        private GridSet _gridSet;
        private Envelope _bounds;
        private int _minZoom;
        private int _maxZoom;
        private List<string> _formats;
        private int _browserCache;
        private bool _disableCache;
        private bool _disableProvider;


        public Layer(string name, string title, GridSet gridset, ICache cache, IProvider provider)
            : this(name, title, gridset, cache, provider, null, 0, 0, null, 0, false, false) { }


        public Layer(string name, string title, GridSet gridset, ICache cache, IProvider provider, Envelope bounds, int minZoom, int maxZoom, List<string> formats, int browserCache, bool DisableCache, bool DisableProvider)
        {
            // validate params
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Layer name cannot be null");
            }

            if (gridset == null)
            {
                throw new ArgumentNullException("Layer GridSet cannot be null");
            }

            if (formats == null && provider == null)
            {
                throw new ArgumentNullException("Layer formats and provider cannot both be null");
            }

            //Set defaults
            if (bounds == null)
            {
                bounds = gridset.Envelope;
            }

            if (maxZoom == 0)
            {
                maxZoom = gridset.Grids.Count;
            }

            if (formats == null)
            {
                formats = provider.GetFormats();
            }

            // Set properties
            this._name = name;
            this._title = title;
            this._cache = cache;
            this._provider = provider;
            this._gridSet = gridset;
            this._bounds = bounds;
            this._minZoom = minZoom;
            this._maxZoom = maxZoom;
            this._formats = formats;
            this._browserCache = browserCache;
            this._disableCache = DisableCache;
            this._disableProvider = DisableProvider;
        }

        
        public string Name { get{ return this._name;} }
        public string Title { get{ return this._title;} }
        public ICache Cache { get{ return this._cache;} }
        public IProvider Provider { get{ return this._provider;} }
        public GridSet Gridset { get{ return this._gridSet;} }
        public Envelope Bounds { get{ return this._bounds;} }
        public int MinZoom { get{ return this._minZoom;} }
        public int MaxZoom { get{ return this._maxZoom;} }
        public List<string> Formats { get{ return this._formats;} }
        public int BrowserCache { get{ return this._browserCache;} }
        public bool DisableCache { get{ return this._disableCache;} }
        public bool DisableProvider { get{ return this._disableProvider;} }

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
    }
}
