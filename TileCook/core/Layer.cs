using System;
using System.Collections.Generic;
using System.Text;

namespace TileCook
{
    public class Layer
    {
        private string _name;
        private string _title;
        private ICache _cache;
        private IProvider _provider;
        private IGridSet _gridSet;

        public Layer(string name, string title, IGridSet gridset,IProvider provider, ICache cache = null)
        {
            // Set name
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Layer Name cannot be null");
            this._name = name;
 
            // Set title
            this._title = title;

            // Set GridSet
            if (gridset == null)
                throw new ArgumentNullException("Layer GridSet cannot be null");
            this._gridSet = gridset;           

            // Set Cache
            this._cache = cache;

            // Set Provider
            if (provider == null)
                throw new ArgumentNullException("Layer Provider cannot be null");
            this._provider = provider;
        }

        
        public string Name { get{ return this._name;} }
        public string Title { get{ return this._title;} }
        public ICache Cache { get{ return this._cache;} }
        public IProvider Provider { get{ return this._provider;} }
        public IGridSet GridSet { get{ return this._gridSet;} }

        public byte[] GetTile(Coord coord, string format)
        {
            // Get envelope 
            // throws TileOutOfRange exception
            Envelope tileEnvelope = this._gridSet.CoordToEnvelope(coord);

            //TO DO: add filter

            // Set coord y value
            // must align coord y value origin with gridset
            coord = this._gridSet.SetY(coord);

            //TO DO: add filter

            byte[] img = null;

            // Get tile from cache?
            if (this.Cache != null)
                img = this.Cache.Get(coord, format);

            // Get tile from provider ?
            if (img == null)
            {
                if (this.Provider is IEnvelopeProvider)
                {
                    IEnvelopeProvider provider = (IEnvelopeProvider)this.Provider;
                    img = provider.Render(tileEnvelope, format, this._gridSet.TileWidth, this._gridSet.TileHeight);
                }
                else if (this.Provider is IPassThoughProvider)
                {
                    IPassThoughProvider provider = (IPassThoughProvider)this.Provider;
                    img = provider.Render(coord, format, this._gridSet.TileWidth, this._gridSet.TileHeight);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected provider type");
                }
                
                // Put tile in cache?
                if (this.Cache != null)
                {
                    this.Cache.Put(coord, format, img);
                }
            }
            return img;
        }
    }
}
