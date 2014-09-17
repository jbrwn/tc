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

        public ProxyProvider(string urlTemplate)
        {
            if (string.IsNullOrEmpty(urlTemplate))
            {
                throw new ArgumentNullException("ProxyProvider url template cannot be null or empty");
            }
            else
            {
                this._urlTemplate = urlTemplate;
            }

            string format = Path.GetExtension(urlTemplate).Substring(1);
            this._formats = new List<string> { format };
        }

        public string UrlTemplate { get { return this._urlTemplate; } }

        public byte[] Render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            string urlRequest = this._urlTemplate;
            urlRequest = Regex.Replace(urlRequest, "{z}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            urlRequest = Regex.Replace(urlRequest,"{x}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            urlRequest = Regex.Replace(urlRequest,"{y}", coord.Z.ToString(), RegexOptions.IgnoreCase);
            Uri uri = new Uri(urlRequest);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

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

        public List<string> GetFormats()
        {
            return this._formats;
        }

        public List<VectorLayerMetadata> GetVectorTileMetadata()
        {
            return new List<VectorLayerMetadata>();
        }
    }
}
