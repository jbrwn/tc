using System.Web.Http.ExceptionHandling;
using System.Web.Http;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

class GlobalExceptionHandler : ExceptionHandler
{
    public override void Handle(ExceptionHandlerContext context)
    {
        //context.Result = context.ExceptionContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "oops!");
        context.Result = new ErrorResult
        {
            Request = context.ExceptionContext.Request,
            Content = "Something bad happend!"
        };
    }

    private class ErrorResult : IHttpActionResult
    {
        public HttpRequestMessage Request { get; set; }

        public string Content { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, Content);
            return Task.FromResult(response);
        }
    }
}