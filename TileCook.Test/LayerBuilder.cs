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

        public LayerBuilder SetCache(ICache cache)
        {
            this._cache = cache;
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

        public LayerBuilder SetBounds(Envelope envelope)
        {
            this._bounds = envelope;
            return this;
        }

        public LayerBuilder SetMaxZoom(int maxZoom)
        {
            this._maxZoom = maxZoom;
            return this;
        }

        // builds bare bones layer object with mocks
        public LayerBuilder Initialize()
        {
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(m => m.GetFormats()).Returns(new List<string>());

            var mockGridSet = new Mock<IGridSet>();
            mockGridSet.Setup(m => m.Resolutions.Count).Returns(1);
            mockGridSet.Setup(m => m.Envelope).Returns(new Envelope(0, 0, 0, 0));

            var mockCache = new Mock<ICache>();

            this._name = "test";
            this._gridSet = mockGridSet.Object;
            this._provider = mockProvider.Object;
            this._cache = mockCache.Object;
            return this;
        }

        public static implicit operator Layer(LayerBuilder obj)
        {
            return obj.Build();
        }

    }
}
