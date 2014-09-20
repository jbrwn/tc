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
    public class ErrorController : ApiController
    {
        [HttpGet, HttpDelete, HttpHead, HttpPatch, HttpOptions, HttpPost, HttpPut]
        public HttpResponseMessage Error404()
        {
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The requested resource does not exist");
        }

    }
}
