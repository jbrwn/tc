using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Test
{
    class MapnikProviderBuilder
    {
        private string _xmlConfig;
        private int _buffer;
        private string _pngOptions;
        private string _jpegOptions;
        private int _gridLayerIndex;
        private int _gridResolution;
        private List<string> _gridFields;
        private string _compression;

        public MapnikProvider Build()
        {
            return new MapnikProvider(
                this._xmlConfig,
                this._buffer,
                this._pngOptions,
                this._jpegOptions,
                this._gridLayerIndex,
                this._gridResolution,
                this._gridFields,
                this._compression
            );
        }

        public MapnikProviderBuilder SetXmlConfig(string xmlConfig)
        {
            this._xmlConfig = xmlConfig;
            return this;
        }

        public static implicit operator MapnikProvider(MapnikProviderBuilder obj)
        {
            return obj.Build();
        }
    }
}
