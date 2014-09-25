using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net;
using System.IO;

namespace TileCook
{
    public class ProxyProvider : IPassThoughProvider, IVectorTileProvider
    {
        private readonly List<string> _formats;
        private readonly string _urlTemplate;
        private readonly List<VectorLayerMetadata> _vectorLayers;

        public ProxyProvider(string urlTemplate, string format, IEnumerable<VectorLayerMetadata> vectorLayers = null)
        {
            if (string.IsNullOrEmpty(urlTemplate))
                throw new ArgumentNullException("ProxyProvider url template cannot be null or empty");
            this._urlTemplate = urlTemplate;

            this._formats = new List<string> { format };
        }

        public string UrlTemplate { get { return this._urlTemplate; } }

        public byte[] Render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            if (!this._formats.Contains(format.ToLower()))
                throw new InvalidTileFormatException(
                    string.Format("Invalid tile FORMAT {0}", format)
                );

            string urlRequest = this._urlTemplate;
            urlRequest = Regex.Replace(urlRequest, "{z}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            urlRequest = Regex.Replace(urlRequest,"{x}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            urlRequest = Regex.Replace(urlRequest,"{y}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            Uri uri = new Uri(urlRequest);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            responseStream.CopyTo(ms);
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw new TileNotFoundException(
                    "The requested tile was not found",
                    ex
                );
            }

        }

        public List<string> GetFormats()
        {
            return new List<string>(this._formats);
        }

        public List<VectorLayerMetadata> GetVectorTileMetadata()
        {
            List<VectorLayerMetadata> vl = new List<VectorLayerMetadata>();
            if (this._vectorLayers != null)
                vl.AddRange(this._vectorLayers);
            return vl;
        }
    }
}
