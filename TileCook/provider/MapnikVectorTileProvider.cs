using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using NETMapnik;
using System.IO;

namespace TileCook
{
    [DataContract]
    class MapnikVectorTileProvider : IPassThoughProvider
    {

        private Map _map;
        private static readonly Object mapLock = new Object();

        private MapnikVectorTileProvider() { }

        public MapnikVectorTileProvider(string xmlConfig, string TileSource)
        {
            this.xmlConfig = xmlConfig;
            this.TileSource = TileSource;
            this.Buffer = 0;

            //set img defaults
            this.pngOptions = "png";
            this.jpegOptions = "jpeg";

            _map = new Map();
            _map.LoadMap(xmlConfig);
        }

        [DataMember(IsRequired = true)]
        public string xmlConfig { get; set; }

        [DataMember(IsRequired = true)]
        public string TileSource{ get; set; }

        [DataMember]
        public int Buffer { get; set; }

        [DataMember]
        public string pngOptions { get; set; }

        [DataMember]
        public string jpegOptions { get; set; }

        public byte[] render(Coord coord, string format, int tileWidth, int tileHeight)
        {
            // Get source layer
            Layer sourceLayer = LayerCache.GetLayer(TileSource);
            byte[] tileBytes = sourceLayer.getTile(coord.z, coord.x, coord.y, "pbf");

            // Flip y coordinate - mapnik vector tile assumes top left origin.
            int flippedY = sourceLayer.FlipY(coord.z, coord.y);
            VectorTile vTile = new VectorTile(coord.z, coord.x, flippedY, Convert.ToUInt32(tileWidth), Convert.ToUInt32(tileHeight));
            vTile.SetBytes(tileBytes);
            Envelope envelope = sourceLayer.gridset.CoordToEnvelope(new Coord(coord.z,coord.x,coord.y));

            // Lock map object for rendering
            // TO DO: better strategy is to create a pool of map objects 
            lock (mapLock)
            {
                _map.Width = Convert.ToUInt32(tileWidth);
                _map.Height = Convert.ToUInt32(tileHeight);
                _map.ZoomToBox(envelope.minx, envelope.miny, envelope.maxx, envelope.maxy);
                _map.Buffer = this.Buffer;
                Image img = new Image(Convert.ToInt32(_map.Width), Convert.ToInt32(_map.Height));
                vTile.Render(_map, img);
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
        }

        public List<string> getFormats()
        {
            return new List<string> { "png", "jpg"};
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (this.jpegOptions == null) { this.jpegOptions = "jpeg"; }
            if (this.pngOptions == null) { this.pngOptions = "png"; }
            //buffer defaults to 0

            if (!Path.IsPathRooted(this.xmlConfig))
            {
                xmlConfig = Path.Combine(LayerCache.ConfigDirectory, this.xmlConfig);
            }
            _map = new Map();
            _map.LoadMap(this.xmlConfig);
        }

    }
}
