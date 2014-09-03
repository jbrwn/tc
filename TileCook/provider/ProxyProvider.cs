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
    [DataContract]
    public class ProxyProvider : IPassThoughProvider
    {
        private ProxyProvider() {}

        private List<string> _formats;

        public ProxyProvider(string urlTemplate)
        {
            this.urlTemplate = urlTemplate;
            string format = Path.GetExtension(urlTemplate).Substring(1);
            this._formats = new List<string> { format };
        }

        [DataMember(IsRequired = true)]
        public string urlTemplate {get; set;}

        public byte[] render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            string urlRequest = this.urlTemplate;
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

        public List<string> getFormats()
        {
            return this._formats;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //validate urlTemplate?

            string format = Path.GetExtension(urlTemplate).Substring(1);
            this._formats = new List<string> { format };
        }

    }
}
