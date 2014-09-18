using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook.Models
{
    public interface ILayerRepository
    {
        Layer Get(string name);
        IEnumerable<Layer> GetAll();
        void Put(Layer layer);
        void Update(Layer layer);
        void Delete(string name);
        void Save();
    }
}
