using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TileCook.Web.TileJSONService;


namespace TileCook.Web.Controllers
{
    public class TileJSONController : ApiController
    {
        [HttpGet]
        [ActionName("GET")]
        public HttpResponseMessage GET()
        {
            TileJSONService.TileJSONService tilejson = new TileJSONService.TileJSONService();
            tilejson.tilejson = "2.1.0";
            tilejson.tiles = new List<string> {  };
            return Request.CreateResponse(HttpStatusCode.OK, tilejson, Configuration.Formatters.JsonFormatter);
        }

    }
}
