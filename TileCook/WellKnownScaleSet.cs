using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Json;
using System.IO;

namespace TileCook
{
    public static class WellKnownScaleSet
    {

        private static ConcurrentDictionary<string, GridSet> _gridSets;

        static WellKnownScaleSet()
        {
            _gridSets = new ConcurrentDictionary<string, GridSet>();
        }
        
        public static void RegisterDirecotry(string directory)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GridSet));

            //deserialize 
            foreach (string file in Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                //deserialize 
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                        GridSet g = (GridSet)jsonSerializer.ReadObject(fs);
                        _gridSets[g.name] = g;
                }
            }
        }

        public static GridSet GetGridSet(string WellKnownScaleSet)
        {
            GridSet g;
            if (_gridSets.TryGetValue(WellKnownScaleSet, out g))
            {
                return g;
            }
            return null;
        }
    }
}
