using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;


namespace TileCook.API
{
    public static class ErrorConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Set global exception handler
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            // Configure catch-all 404 route
            config.Routes.MapHttpRoute(
                name: "DefaultCatchall",
                routeTemplate: "{*url}",
                defaults: new
                {
                    controller = "Error",
                    action = "Error404"
                });
        }
    }
}
