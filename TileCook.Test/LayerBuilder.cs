using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace TileCook.Test
{
    public class LayerBuilder
    {
        private string _name;
        private string _title;
        private ICache _cache;
        private IProvider _provider;
        private IGridSet _gridSet;
        private Envelope _bounds;
        private int _minZoom;
        private int _maxZoom;
        private List<string> _formats;
        private int _browserCache;
        private bool _disableCache;
        private bool _disableProvider;

        public Layer Build()
        {
            return new Layer(
                this._name,
                this._title,
                this._gridSet,
                this._cache,
                this._provider,
                this._bounds,
                this._minZoom,
                this._maxZoom,
                this._formats,
                this._browserCache,
                this._disableCache,
                this._disableProvider
            );
        }

        public LayerBuilder SetName(string name)
        {
            this._name = name;
            return this;
        }

        public LayerBuilder SetGridSet(IGridSet gridSet)
        {
            this._gridSet = gridSet;
            return this;
        }

        public LayerBuilder SetProvider(IProvider provider)
        {
            this._provider = provider;
            return this;
        }

        public LayerBuilder SetFormats(List<string> formats)
        {
            this._formats = formats;
            return this;
        }

        public static implicit operator Layer(LayerBuilder obj)
        {
            return obj.Build();
        }

    }
}
