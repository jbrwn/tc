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
    
    public class MapnikVectorTileProvider : IPassThoughProvider
    {

        private Map _map;
        private static readonly Object mapLock = new Object();
        private Layer _tileSource;
        private string _pngOptions;
        private string _jpegOptions;

        public MapnikVectorTileProvider(string xmlConfig, Layer tileSource)
            : this(xmlConfig, tileSource, 0, null, null) {}

        public MapnikVectorTileProvider(string xmlConfig, Layer tileSource, int buffer, string pngOptions, string jpegOptions)
        {
            if (string.IsNullOrEmpty(xmlConfig))
            {
                throw new ArgumentNullException("MapnikVectorProvider xmlConfig cannot be null or empty");
            }

            if (tileSource == null )
            {
                throw new ArgumentNullException("MapnikVectorProvider tileSource cannot be null");
            }
            else
            {
                this._tileSource = tileSource;
            }

            // set jpegOptions
            if (jpegOptions == null)
            {
                this._jpegOptions = "jpeg";
            }
            else
            {
                this._jpegOptions = jpegOptions;
            }

            // set pngOptions
            if (pngOptions == null)
            {
                this._pngOptions = "png";
            }
            else
            {
                this._pngOptions = pngOptions;
            }

            this._map = new Map();
            _map.LoadMap(xmlConfig);
            this._map.Buffer = buffer;
        }

        public string JpegOptions { get { return this._jpegOptions; } }
        public string PngOptions { get { return this._pngOptions; } }

        public byte[] Render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            byte[] tileBytes = this._tileSource.GetTile(coord, "pbf");
            
            // Uncompress bytes
            if (tileBytes.Length > 0)
            {
                tileBytes = Decompress(tileBytes);
            }
            
            // Set vector tile bytes
            // mapnik vector tile assumes top left origin
            int y = coord.Y;
            if (!coord.TopOrigin)
                y = this._tileSource.GridSet.GridHeight(coord.Z) - coord.Y - 1;

            VectorTile vTile = new VectorTile(coord.Z, coord.X, y, Convert.ToUInt32(tileWidth), Convert.ToUInt32(tileHeight));
            vTile.SetBytes(tileBytes);

            // Get coord envelope
            Envelope envelope = this._tileSource.GridSet.CoordToEnvelope(coord);

            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.Minx, envelope.Miny, envelope.Maxx, envelope.Maxy);
                Image img = new Image(Convert.ToInt32(_map.Width), Convert.ToInt32(_map.Height));
                vTile.Render(_map, img);

                format = format.ToLower();
                if (format == "png" || format == "jpg")
                {
                    if (format == "png")
                    {
                        format = this._pngOptions;
                    }
                    if (format == "jpg")
                    {
                        format = this._jpegOptions;
                    }
                    return img.Encode(format);
                }
                // Format not expected so throw exception
                throw new InvalidTileFormatException(
                    string.Format("Invalid tile FORMAT {0}", format)
                );

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
    }
}
