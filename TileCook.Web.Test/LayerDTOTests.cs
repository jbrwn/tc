using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TileCook.Web.Models;

namespace TileCook.Web.Test
{
    [TestClass]
    public class LayerDTOTests
    {
        [TestMethod]
        public void Deserialize_Json_PropertiesSet()
        {
            string json = @"{
                'Name': 'test'
            }";

            LayerDTO dto = JsonConvert.DeserializeObject<LayerDTO>(json);
        }
    }
}
