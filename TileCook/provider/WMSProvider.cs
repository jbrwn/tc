using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net;
using System.Web;
using System.IO;

namespace TileCook
{
    [DataContract]
    public class WMSProvider : IEnvelopeProvider
    {
        private WMSProvider() { }

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format)
            : this(url, version, layers, crs, styles, format, null, null, null) {}

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld)
            : this(url, version, layers, crs, styles, format, sld, null, null) {}

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld, string bgcolor)
            : this(url, version, layers, crs, styles, format, sld, bgcolor, null) { }

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld, string bgcolor, string transparent)
        {
            this.url = url;
            this.version = version;
            this.layers = layers;
            this.crs = crs;
            this.styles = styles;
            this.format = format;
            this.sld = sld;
            this.bgcolor = bgcolor;
            this.transparent = transparent;
        }

        [DataMember(IsRequired = true)]
        public string url { get; set; }
        [DataMember(IsRequired = true)]
        public string version { get; set; }
        [DataMember(IsRequired = true)]
        public string layers { get; set; }
        [DataMember(IsRequired = true)]
        public string crs { get; set; }
        [DataMember(IsRequired = true)]
        public string styles { get; set; }
        [DataMember(IsRequired = true)]
        public string format { get; set; }

        [DataMember]
        public string sld { get; set; }
        [DataMember]
        public string bgcolor { get; set; }
        [DataMember]
        public string transparent { get; set; }

        public byte[] Render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            StringBuilder query = new StringBuilder();
            query.Append("request=GetMap");
            query.Append("&service=WMS");
            query.Append("&version=" + HttpUtility.UrlEncode(this.version));
            query.Append("&layers=" + HttpUtility.UrlEncode(this.layers));
            query.Append("&styles=" + HttpUtility.UrlEncode(this.styles));
            if (version.Equals("1.3.0")) { query.Append("&crs=" + HttpUtility.UrlEncode(this.crs)); }
            else { query.Append("&srs=" + HttpUtility.UrlEncode(this.crs)); }
            query.Append("&bbox=" + envelope.Minx.ToString() + "," + envelope.Miny.ToString() + "," + envelope.Maxx.ToString() + "," + envelope.Maxy.ToString());
            query.Append("&width=" + tileWidth.ToString());
            query.Append("&height=" + tileHeight.ToString());
            query.Append("&format=" + HttpUtility.UrlEncode(format));
            if (this.sld != null) { query.Append("&sld=" + HttpUtility.UrlEncode(this.sld)); }
            if (this.bgcolor != null) { query.Append("&bgcolor=" + HttpUtility.UrlEncode(this.bgcolor)); }    
            if (this.transparent != null) { query.Append("&transparent=" + HttpUtility.UrlEncode(this.transparent)); }

            UriBuilder ub = new UriBuilder(this.url);
            ub.Query = query.ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ub.Uri);
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
            return new List<string> { this.format };
        }
    }
}
