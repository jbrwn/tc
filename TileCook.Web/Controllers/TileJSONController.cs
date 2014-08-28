using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TileCook.Web.TileJSONService;


namespace TileCook.Web.Controllers
{
    public class TileJSONController : ApiController
    {
        [HttpGet]
        [ActionName("GetTileJSON")]
        public HttpResponseMessage GetTileJSON(string layer)
        {

            //string json = @"{""bounds"":[-180,-85.0511,180,85.0511],""center"":[-41.484375,-46.8000594467873,1],""description"":"""",""filesize"":213730,""format"":""pbf"",""id"":""joelsbrown1.ffb527f1"",""maxzoom"":6,""minzoom"":0,""name"":""test_source"",""private"":false,""scheme"":""xyz"",""tilejson"":""2.0.0"",""tiles"":[""http://a.tiles.mapbox.com/v4/joelsbrown1.ffb527f1/{z}/{x}/{y}.vector.pbf?access_token=pk.eyJ1Ijoiam9lbHNicm93bjEiLCJhIjoidS1rR3FRdyJ9.08WBnyimBMtxAY8r6DnU-Q"",""http://b.tiles.mapbox.com/v4/joelsbrown1.ffb527f1/{z}/{x}/{y}.vector.pbf?access_token=pk.eyJ1Ijoiam9lbHNicm93bjEiLCJhIjoidS1rR3FRdyJ9.08WBnyimBMtxAY8r6DnU-Q""],""vector_layers"":[{""description"":"""",""fields"":{""FID_ne_10m"":""Number"",""adm0_a3_l"":""String"",""adm0_a3_r"":""String"",""adm0_left"":""String"",""adm0_right"":""String"",""adm0_usa"":""Number"",""comment"":""String"",""featurecla"":""String"",""labelrank"":""Number"",""name"":""String"",""note_"":""String"",""scalerank"":""Number"",""sov_a3_l"":""String"",""sov_a3_r"":""String"",""type"":""String""},""id"":""ne_10m_admin_0_boundary_lines_land""}]}";
            string json = @"{""bounds"":[-180,-85.0511,180,85.0511],
                        ""center"":[-41.484375,-46.8000594467873,1],
                        ""description"":"""",""filesize"":213730,
                        ""format"":""pbf"",
                        ""id"":""joelsbrown1.ffb527f1"",
                        ""maxzoom"":6,
                        ""minzoom"":0,
                        ""name"":""test_source"",
                        ""private"":false,
                        ""scheme"":""tms"",
                        ""tilejson"":""2.0.0"",
                        ""tiles"":[""http://localhost:49414/Services/TMS/1.0.0/world_merc/{z}/{x}/{y}.pbf""],
                        ""vector_layers"":[{""id"":""world_merc"",
                            ""description"":""world_merc_test"",
                            ""fields"":{""FIPS"":""String"",""ISO2"":""String"",""ISO3"":""String"",""UN"":""Number"",""NAME"":""String"",""AREA"":""Number"",""POP2005"":""Number"",""REGION"":""Number"",""SUBREGION"":""Number"",""LON"":""Number"",""LAT"":""Number""}
                        }]
            }";
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;

            response.Content = new StringContent(json);
            string mimeMapping = ContentType.GetContentType("json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeMapping);
            return response;

            // Validate layer
            Layer tileLayer = LayerCache.GetLayer(layer);
            if (tileLayer == null)
            {
                string message = "Tileset does not exist";
                return Request.CreateResponse(HttpStatusCode.NotFound, message, Configuration.Formatters.JsonFormatter);
            }

            //TileJSON end point must have at least one tiles object 
            List<string> tiles = GetTileList(tileLayer);
            if (tiles == null)
            {
                string message = "Tileset does not exist";
                return Request.CreateResponse(HttpStatusCode.NotFound, message, Configuration.Formatters.JsonFormatter);
            }

            // Create tilejson object
            TileJSON tilejson = new TileJSON();
            
            //Required
            tilejson.tilejson = "2.1.0";
            tilejson.tiles = tiles;

            //optional
            tilejson.name = tileLayer.name;
            tilejson.descritpion = tileLayer.Title;
            tilejson.scheme = "tms";
            tilejson.minzoom = tileLayer.minZoom;
            tilejson.maxzoom = tileLayer.maxZoom;
            tilejson.grids = GetGridList(tileLayer);

            return Request.CreateResponse(HttpStatusCode.OK, tilejson, Configuration.Formatters.JsonFormatter);
        }

        private List<string> GetTileList(Layer layer)
        {
            foreach (string format in layer.formats)
            {
                if (format.ToLower() != "json")
                {
                    return new List<string>() {HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = layer.name, Z = "{z}", X = "{x}", Y = "{y}", Format = format }))};
                }
            }
            return null;
        }

        private List<string> GetGridList(Layer layer)
        {
            foreach (string format in layer.formats)
            {
                if (format.ToLower() == "json")
                {
                    return new List<string>() { HttpUtility.UrlDecode(Url.Link("TMSTile", new { Version = "1.0.0", TileMap = layer.name, Z = "{z}", X = "{x}", Y = "{y}", Format = format })) };
                }
            }
            return null;
        }
    }
}
