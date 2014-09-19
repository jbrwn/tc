using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using TileCook.API.Models;
using System.Net.Http.Formatting;

namespace TileCook.API.Filters
{
    public class TMSExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is InvalidRequestParameterException
                || context.Exception is TileNotFoundException
                || context.Exception is InvalidTileFormatException
                || context.Exception is TileOutOfRangeException)
            {
                TileMapServiceError error = new TileMapServiceError();
                error.Message = context.Exception.Message;
                context.Response = context.Request.CreateResponse(HttpStatusCode.NotFound, error, new XmlMediaTypeFormatter() { UseXmlSerializer = true });
            }
        }
    }
}
