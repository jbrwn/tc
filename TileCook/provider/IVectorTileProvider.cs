using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileCook
{
    public interface IVectorTileProvider
    {
        List<VectorLayerMetadata> GetVectorTileMetadata();
    }

    public class VectorLayerMetadata
    {
        public VectorLayerMetadata() 
        {
            this.fields = new Dictionary<string, string>();
        }

        public string id;
        public string descritpion;
        public Dictionary<string,string> fields;
    }
}
