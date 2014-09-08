using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using NETMapnik;
using System.IO;
using Ionic.Zlib;

namespace TileCook
{
    [DataContract]
    public class MapnikVectorTileProvider : IPassThoughProvider
    {

        private Map _map;
        private static readonly Object mapLock = new Object();

        private MapnikVectorTileProvider() { }

        public MapnikVectorTileProvider(string xmlConfig, string TileSource)
        {
            this.XmlConfig = xmlConfig;
            this.TileSource = TileSource;
            this.Buffer = 0;

            //set img defaults
            this.pngOptions = "png";
            this.jpegOptions = "jpeg";

            _map = new Map();
            _map.LoadMap(xmlConfig);
        }

        [DataMember(IsRequired = true)]
        public string XmlConfig { get; set; }

        [DataMember(IsRequired = true)]
        public string TileSource{ get; set; }

        [DataMember]
        public int Buffer { get; set; }

        [DataMember]
        public string pngOptions { get; set; }

        [DataMember]
        public string jpegOptions { get; set; }

        public byte[] Render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            // Get source layer
            Layer sourceLayer = LayerCache.GetLayer(TileSource);
            byte[] tileBytes = sourceLayer.GetTile(coord.Z, coord.X, coord.Y, "pbf");
            
            // Uncompress bytes
            if (tileBytes.Length > 0)
            {
                tileBytes = Decompress(tileBytes);
            }

            // Flip y coordinate - mapnik vector tile assumes top left origin.
            int flippedY = sourceLayer.FlipY(coord.Z, coord.Y);
            VectorTile vTile = new VectorTile(coord.Z, coord.X, flippedY, Convert.ToUInt32(tileWidth), Convert.ToUInt32(tileHeight));
            vTile.SetBytes(tileBytes);
            Envelope envelope = sourceLayer.Gridset.CoordToEnvelope(new Coord(coord.Z,coord.X,coord.Y));

            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.Minx, envelope.Miny, envelope.Maxx, envelope.Maxy);
                _map.Buffer = this.Buffer;
                Image img = new Image(Convert.ToInt32(_map.Width), Convert.ToInt32(_map.Height));
                vTile.Render(_map, img);

                if (format == "png" || format == "jpg")
                {
                    if (format == "png")
                    {
                        format = this.pngOptions;
                    }
                    if (format == "jpg")
                    {
                        format = this.jpegOptions;
                    }
                    return img.Encode(format);
                }
                // Format not expected so throw exception
                throw new InvalidOperationException(string.Format("Format {0} not expected", format));

            }
        }

        public List<string> GetFormats()
        {
            return new List<string> { "png", "jpg"};
        }

        private byte[] Decompress(byte[] bytes)
        {
            if (bytes[0] == 0x1f && bytes[1] == 0x8b)
            {
                using (MemoryStream memory = new MemoryStream(bytes))
                {
                    using (GZipStream gzip = new GZipStream(memory, CompressionMode.Decompress))
                    {
                        using (MemoryStream result = new MemoryStream())
                        {
                            gzip.CopyTo(result);
                            return result.ToArray();
                        }
                    }
                }
            }
            if (bytes[0] == 0x78 && bytes[1] == 0x9c)
            {
                using (MemoryStream memory = new MemoryStream(bytes))
                {
                    using (ZlibStream deflate = new ZlibStream(memory, CompressionMode.Decompress))
                    {
                        using (MemoryStream result = new MemoryStream())
                        {
                            deflate.CopyTo(result);
                            return result.ToArray();
                        }
                    }
                }
            }
            return bytes;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (this.jpegOptions == null) { this.jpegOptions = "jpeg"; }
            if (this.pngOptions == null) { this.pngOptions = "png"; }
            //buffer defaults to 0

            if (!Path.IsPathRooted(this.XmlConfig))
            {
                this.XmlConfig = Path.Combine(LayerCache.ConfigDirectory, this.XmlConfig);
            }
            _map = new Map();
            _map.LoadMap(this.XmlConfig);
        }

    }
}
