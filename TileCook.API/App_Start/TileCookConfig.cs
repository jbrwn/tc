using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TileCook.Models;
using Newtonsoft.Json;
using System.IO;

namespace TileCook.API
{
    public static class TileCookConfig
    {
        public static void LoadConfigs(ILayerRepository repository, string configDir)
        {
            // Create configuration object map
            IPathResolver pathResolver = new PathResolver(configDir);
            LayerMap LayerMap = new LayerMap(
                new GridSetMap(
                    new EnvelopeMap()
                ),
                new CacheMap(pathResolver),
                new ProviderMap(
                    new VectorTileLayerMap(
                        new GridSetMap(
                            new EnvelopeMap()
                        ),
                        new CacheMap(pathResolver),
                        new VectorTileProviderMap(pathResolver),
                        new EnvelopeMap()
                    ),
                    pathResolver
                ),
                new EnvelopeMap()
            );

            // Deserialize config files
            JsonSerializer serializer = new JsonSerializer();
            try
            {
                foreach (string file in Directory.EnumerateFiles(configDir, "*.json", SearchOption.TopDirectoryOnly))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        using (JsonTextReader reader = new JsonTextReader(sr))
                        {
                            LayerConfig LayerConfig = (LayerConfig)serializer.Deserialize(reader, typeof(LayerConfig));
                            Layer l = LayerMap.Map(LayerConfig);
                            repository.Put(l);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceWarning()
            }

        }
    }
}
