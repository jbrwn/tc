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

        public HttpResponseMessage Get()
        {
            // Start response
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(@"
 _____  _  _          ___               _    
/__   \(_)| |  ___   / __\ ___    ___  | | __
  / /\/| || | / _ \ / /   / _ \  / _ \ | |/ /
 / /   | || ||  __// /___| (_) || (_) ||   < 
 \/    |_||_| \___|\____/ \___/  \___/ |_|\_\
            ");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;

        }
    }
}
