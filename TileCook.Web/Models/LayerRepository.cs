using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;

namespace TileCook.Web.Models
{
    public class LayerRepository : ILayerRepository
    {

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
    }
}