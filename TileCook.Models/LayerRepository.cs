using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.IO;

namespace TileCook.Models
{
    public class LayerRepository : ILayerRepository
    {
        private static ConcurrentDictionary<string, Layer> _layers = new ConcurrentDictionary<string, Layer>();

        public LayerRepository() {}

        public Layer Get(string name)
        {
            Layer layer = null;
            _layers.TryGetValue(name, out layer);
            return layer;
        }

        public IEnumerable<Layer> GetAll()
        {
            return _layers.Values.ToList<Layer>();
        }

        public void Put(Layer layer)
        {
            if (!_layers.TryAdd(layer.Name, layer))
            {
                throw new InvalidOperationException(string.Format("Put failed for layer {0}", layer.Name));
            }
        }

        public void Update(Layer layer)
        {
            throw new NotImplementedException();
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        //// convenience method for initial load of _layers dictionary
        //// should only be called when app starts for first time
        //public void Load(TextReader textReader)
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    using (JsonTextReader reader = new JsonTextReader(textReader))
        //    {
        //        LayerConfig LayerConfig = (LayerConfig)serializer.Deserialize(reader);
        //        // map LayerConfig to layer object
        //        //_layers.add
        //    }
        //}
    }
}