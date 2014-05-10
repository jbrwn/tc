using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace TileCook
{
    public static class LayerCache
    {

        private static ConcurrentDictionary<string, Layer> _layers;
        private static DataContractJsonSerializer _jsonSerializer;
        private static string _configDirectory;

        static LayerCache()
        {
            _layers = new ConcurrentDictionary<string, Layer>();
            _configDirectory = string.Empty;
            _jsonSerializer = new DataContractJsonSerializer(
               typeof(Layer),
               new List<Type>{
                    typeof(DiskCache),
                    typeof(NoCache),
                    typeof(MapnikProvider),
                    typeof(TestProvider),
                    typeof(ProxyProvider),
                    typeof(WMSProvider)
                }
           );
        }

        //Use config directory to resolve relative paths in config files
        public static string ConfigDirectory
        {
            get { return _configDirectory; }
            set { _configDirectory = value; }
        }

        public static void RegisterDirectory(string directory)
        {   
            foreach (string file in Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
            {
                //deserialize 
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    Layer l = (Layer)_jsonSerializer.ReadObject(fs);
                    _layers[l.name] = l;
                }
            }

        }

        public static void RegisterFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                Layer l = (Layer)_jsonSerializer.ReadObject(fs);
                _layers[l.name] = l;
            }
        }

        public static void SaveToFile(Layer layer, string path)
        {
            //serialize 
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                _jsonSerializer.WriteObject(fs, layer);
            }
        }

        public static Layer GetLayer(string name)
        {
            Layer layer;
            if (_layers.TryGetValue(name, out layer))
            {
                return layer;
            }
            return null;
        }

        public static Dictionary<string, Layer> GetLayers()
        {
            return new Dictionary<string, Layer>(_layers);
        }
    }
}
