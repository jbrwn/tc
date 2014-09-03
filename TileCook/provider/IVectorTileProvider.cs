using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TileCook
{
    public interface IVectorTileProvider
    {
        List<VectorLayerMetadata> GetVectorTileMetadata();
    }

    public class VectorLayerMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
