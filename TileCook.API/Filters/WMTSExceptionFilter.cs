using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using TileCook.API.Models;
using TileCook.API.Formatting;
using System.Net.Http.Formatting;
using System.Xml.Serialization;

namespace TileCook.API.Filters
{
    public class WMTSExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is InvalidRequestParameterException)
            {
                InvalidRequestParameterException ex = context.Exception as InvalidRequestParameterException;
                context.Response = OWSExceptionReport(
                    context,
                    HttpStatusCode.BadRequest,
                    "InvalidParameterValue",
                    ex.Message,
                    ex.ParamName
                );
            }
            else if (context.Exception is InvalidTileFormatException)
            {
                InvalidTileFormatException ex = context.Exception as InvalidTileFormatException;
                context.Response = OWSExceptionReport(
                    context,
                    HttpStatusCode.BadRequest,
                    "InvalidParameterValue",
                    ex.Message,
                    "Format"
                );
            }
            else if (context.Exception is TileOutOfRangeException)
            {
                TileOutOfRangeException ex = context.Exception as TileOutOfRangeException;
                string locator = null;
                if (ex.ParamName != null)
                {
                    switch (ex.ParamName.ToLower())
                    {
                        case "z":
                            locator = "TileMatrix";
                            break;
                        case "x":
                            locator = "TileCol";
                            break;
                        case "y":
                            locator = "TileRow";
                            break;
                    }
                }

                context.Response = OWSExceptionReport(
                    context,
                    HttpStatusCode.BadRequest,
                    "TileOutOfRange",
                    ex.Message,
                    locator
                );
            }
            else if (context.Exception is TileNotFoundException)
            {
                TileNotFoundException ex = context.Exception as TileNotFoundException;
                context.Response = OWSExceptionReport(
                    context,
                    HttpStatusCode.NotFound,
                    "TileNotFound",
                    ex.Message,
                    null
                );
            }
        }

        private HttpResponseMessage OWSExceptionReport(HttpActionExecutedContext context, HttpStatusCode statusCode, string code, string message, string locator)
        {
            ExceptionReport ex = new ExceptionReport();
            ex.lang = "en";
            ex.version = "1.0.0";
            ex.Exception = new ExceptionType[] { new ExceptionType() };
            ex.Exception[0].ExceptionText = new string[] { message };
            ex.Exception[0].exceptionCode = code;
            ex.Exception[0].locator = locator;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("ows", "http://www.opengis.net/ows/1.1");
            return context.Request.CreateResponse(statusCode, ex, new ExtendedXmlMediaTypeFormatter(ns));
        }
    }
}
