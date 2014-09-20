using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace TileCook.API.Controllers
{
    public class DefaultController : ApiController
    {

        public string Get()
        {
            return "TileCook tile server!";
        }
    }
}
