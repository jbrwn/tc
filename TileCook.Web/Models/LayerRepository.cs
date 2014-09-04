using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace TileCook.Web.Models
{
    public class LayerRepository : ILayerRepository
    {
        private static ConcurrentDictionary<string, Layer> _layers = new ConcurrentDictionary<string, Layer>();

        public LayerRepository() {}

        public Layer Get(string name)
        {
            return LayerCache.GetLayer(name);
        }

        public IEnumerable<Layer> GetAll()
        {
            return LayerCache.GetLayers();
        }

        public void Put(Layer layer)
        {
            throw new NotImplementedException();
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

        // convenience method for initial load of _layers dictionary
        // should only be called when app starts for first time
        public void Load(TextReader textReader)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (JsonTextReader reader = new JsonTextReader(textReader))
            {
                LayerDTO layerDTO = (LayerDTO)serializer.Deserialize(reader);
                // map layerDTO to layer object
                //_layers.add
            }
        }
    }
}