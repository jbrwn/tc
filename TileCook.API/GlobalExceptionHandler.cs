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
            HttpResponseMessage response =
                             new HttpResponseMessage(HttpStatusCode.InternalServerError);
            response.Content = new StringContent(Content);
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }
}