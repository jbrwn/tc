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
    public class WMSProvider : IEnvelopeProvider
    {
        private string _url;
        private string _version;
        private string _layers;
        private string _crs;
        private string _styles;
        private string _format;
        private string _sld;
        private string _bgcolor;
        private string _transparent;

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format)
            : this(url, version, layers, crs, styles, format, null, null, null) {}

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld)
            : this(url, version, layers, crs, styles, format, sld, null, null) {}

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld, string bgcolor)
            : this(url, version, layers, crs, styles, format, sld, bgcolor, null) { }

        public WMSProvider(string url, string version, string layers, string crs, string styles, string format, string sld, string bgcolor, string transparent)
        {
            // Set Url
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("WMSProvider Url cannot be null or empty");
            } 
            else 
            {
                this._url = url;
            }

            // Set Version
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("WMSProvider Version cannot be null or empty");
            } 
            else 
            {
                this._version = version;
            }

            // Set Layers
            if (string.IsNullOrEmpty(layers))
            {
                throw new ArgumentNullException("WMSProvider Layers cannot be null or empty");
            }
            else
            {
                this._layers = layers;
            }

            // Set Crs
            if (string.IsNullOrEmpty(crs))
            {
                throw new ArgumentNullException("WMSProvider CRS cannot be null or empty");
            }
            else
            {
                this._crs = crs;
            }

            // Set Styles
            if (string.IsNullOrEmpty(styles))
            {
                throw new ArgumentNullException("WMSProvider Styles cannot be null or empty");
            }
            else
            {
                this._styles = styles;
            }

            // Set Format
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException("WMSProvider Format cannot be null or empty");
            }
            else
            {
                this._format = format;
            }

            // Set optional properties
            this._sld = sld;
            this._bgcolor = bgcolor;
            this._transparent = transparent;
        }


        public string Url { get { return this._url; } }
        public string Version { get { return this._version; } }
        public string Layers { get { return this._layers; } }
        public string Crs { get { return this._crs; } }
        public string Styles { get { return this._styles; } }
        public string Format { get { return this._format; } }
        public string Sld { get { return this._sld; } }
        public string Bgcolor { get { return this._bgcolor; } }
        public string Transparent { get { return this._transparent; } }

        public byte[] Render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            StringBuilder query = new StringBuilder();
            query.Append("request=GetMap");
            query.Append("&service=WMS");
            query.Append("&version=" + HttpUtility.UrlEncode(this._version));
            query.Append("&layers=" + HttpUtility.UrlEncode(this._layers));
            query.Append("&styles=" + HttpUtility.UrlEncode(this._styles));
            if (this._version.Equals("1.3.0")) { query.Append("&crs=" + HttpUtility.UrlEncode(this._crs)); }
            else { query.Append("&srs=" + HttpUtility.UrlEncode(this._crs)); }
            query.Append("&bbox=" + envelope.Minx.ToString() + "," + envelope.Miny.ToString() + "," + envelope.Maxx.ToString() + "," + envelope.Maxy.ToString());
            query.Append("&width=" + tileWidth.ToString());
            query.Append("&height=" + tileHeight.ToString());
            query.Append("&format=" + HttpUtility.UrlEncode(format));
            if (this._sld != null) { query.Append("&sld=" + HttpUtility.UrlEncode(this._sld)); }
            if (this._bgcolor != null) { query.Append("&bgcolor=" + HttpUtility.UrlEncode(this._bgcolor)); }    
            if (this._transparent != null) { query.Append("&transparent=" + HttpUtility.UrlEncode(this._transparent)); }

            UriBuilder ub = new UriBuilder(this._url);
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
            return new List<string> { this._format };
        }
    }
}
