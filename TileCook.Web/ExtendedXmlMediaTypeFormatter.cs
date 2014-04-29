using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http.Internal;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;




namespace TileCook.Web.Formatting
{
    public class ExtendedXmlMediaTypeFormatter : MediaTypeFormatter
    {
        private XmlWriterSettings _writerSettings;
        private XmlSerializerNamespaces _namespaces;

        public ExtendedXmlMediaTypeFormatter(XmlSerializerNamespaces namespaces)
            : this(namespaces, null) { }
        
        public ExtendedXmlMediaTypeFormatter(XmlSerializerNamespaces namespaces, XmlWriterSettings writerSettings)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
            SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true));
            this._writerSettings = writerSettings;
            this._namespaces = namespaces; 
        }
        
        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (writeStream == null) throw new ArgumentNullException("writeStream");

            var tcs = new TaskCompletionSource<object>();
            try
            {
                if (_writerSettings == null)
                {
                    this._writerSettings = new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true,
                        Indent = true,
                        Encoding = SelectCharacterEncoding(content != null ? content.Headers : null),
                        CloseOutput = false
                    };
                }
                using (XmlWriter writer = XmlWriter.Create(writeStream, _writerSettings))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(type);
                    xmlSerializer.Serialize(writer, value, _namespaces);
                    tcs.SetResult(null);
                }
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
            return tcs.Task;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }
    }
}