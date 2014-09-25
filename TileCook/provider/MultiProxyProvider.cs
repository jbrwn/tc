using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.provider
{
    public class MultiProxyProvider : IPassThoughProvider, IVectorTileProvider
    {
        public readonly Dictionary<string, ProxyProvider> _proxyDictionary;

        public MultiProxyProvider(IDictionary<string, ProxyProvider> proxys)
        {
            if (proxys == null)
                throw new ArgumentNullException("MultiProxy proxy dictionary cannot be null");

            foreach (KeyValuePair<string, ProxyProvider> kvp in proxys)
            {
                this._proxyDictionary[kvp.Key.ToLower()] = kvp.Value;
            }
        }

        public byte[] Render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            ProxyProvider proxy;
            if (!this._proxyDictionary.TryGetValue(format.ToString(), out proxy))
                throw new InvalidTileFormatException(
                    string.Format("Invalid tile FORMAT {0}", format)
                );

            return proxy.Render(coord, format, tileWidth, tileHeight);
        }

        public List<string> GetFormats()
        {
            return this._proxyDictionary.Keys.ToList();
        }

        public List<VectorLayerMetadata> GetVectorTileMetadata()
        {
            ProxyProvider proxy;
            List<VectorLayerMetadata> vl = new List<VectorLayerMetadata>();
            if (this._proxyDictionary.TryGetValue("pbf", out proxy))
                vl.AddRange(proxy.GetVectorTileMetadata());
            return vl;
        }
    }
}
