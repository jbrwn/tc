﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NETMapnik;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TileCook
{
    [DataContract]
    public class MapnikProvider : IEnvelopeProvider
    {

        private Map _map;
        private static readonly Object mapLock = new Object();

        private MapnikProvider() { }
        
        public MapnikProvider(string xmlConfig)
        {
            this.xmlConfig = xmlConfig;

            //set defaults
            this.pngOptions = "png";
            this.jpegOptions = "jpeg";
            this.gridLayerIndex = 0;
            this.gridResolution = 4;
            this.gridFields = new List<string>();
            
            _map = new Map();
            _map.LoadMap(xmlConfig);
        }

        [DataMember(IsRequired=true)]
        public string xmlConfig { get; set; }

        [DataMember]
        public string pngOptions { get; set; }

        [DataMember]
        public string jpegOptions { get; set; }

        [DataMember]
        public int gridLayerIndex { get; set; }

        [DataMember]
        public List<string> gridFields { get; set; }

        [DataMember]
        public int gridResolution { get; set; }

        public byte[] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            //short circut and return null if format is not supported
            if (format.Equals("png", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("jpg", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                // Lock map object for rendering
                // TO DO: better strategy is to create a pool of map objects 
                lock (mapLock)
                {
                    _map.Width = Convert.ToUInt32(tileWidth);
                    _map.Height = Convert.ToUInt32(tileHeight);
                    _map.ZoomToBox(envelope.minx, envelope.miny, envelope.maxx, envelope.maxy);
                    if (format.Equals("png", StringComparison.OrdinalIgnoreCase))
                    {
                        return _map.SaveToBytes(this.pngOptions);
                    }
                    if (format.Equals("jpg", StringComparison.OrdinalIgnoreCase))
                    {
                        return _map.SaveToBytes(this.jpegOptions);
                    }
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        NETMapnik.Grid g = new NETMapnik.Grid(_map.Width, _map.Height);
                        _map.RenderLayer(g, Convert.ToUInt32(this.gridLayerIndex), this.gridFields);
                        string json = JsonConvert.SerializeObject(g.Encode("utf", true,  Convert.ToUInt32(this.gridResolution)));
                        return Encoding.UTF8.GetBytes(json);
                    }
                }
            }
            return null;
        }

        public List<string> getFormats()
        {
            return new List<string>{"png", "jpg", "json"};
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (this.jpegOptions == null) { this.jpegOptions = "jpeg"; }
            if (this.pngOptions == null) { this.pngOptions = "png"; }
            if (this.gridResolution == 0) { this.gridResolution = 4; }
            if (this.gridFields == null) { this.gridFields = new List<string>(); }
            //gridLayerIndex defaults to 0
            
            if (!Path.IsPathRooted(this.xmlConfig))
            {
                xmlConfig = Path.Combine(LayerCache.ConfigDirectory,this.xmlConfig);
            }
            _map = new Map();
            _map.LoadMap(this.xmlConfig);
        }

        public static void RegisterDatasources(string path)
        {
            DatasourceCache.RegisterDatasources(path);
        }

        public static void RegisterFonts(string path)
        {
            FreetypeEngine.RegisterFonts(path, false);
        }
    }
}
