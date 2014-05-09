using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net;
using System.IO;

namespace TileCook
{
    [DataContract]
    class ProxyProvider : IPassThoughProvider
    {
        private ProxyProvider() {}

        private List<string> _formats;

        [DataMember(IsRequired = true)]
        public string urlTemplate;

        public byte[] render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            //string url = "http://a.tile.openstreetmap.org/0/0/0.png";
            StringBuilder sb = new StringBuilder(urlTemplate.ToLower());
            sb.Replace("{z}", coord.z.ToString());
            sb.Replace("{x}", coord.x.ToString());
            sb.Replace("{y}", coord.y.ToString());
            Uri uri = new Uri(sb.ToString());


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
            string format = Path.GetExtension(urlTemplate).Substring(1);
            this._formats = new List<string> { format };
        }

    }
}
