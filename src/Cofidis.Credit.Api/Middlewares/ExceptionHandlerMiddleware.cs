using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cofidis.Credit.Api.Middlewares
{
    public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, IWebHostEnvironment webHostEnvironment) : IMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger = logger;
        private readonly IWebHostEnvironment _environment = webHostEnvironment;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error: {e}", e);
                await HandleException(context, e);
            }
        }

        public Task HandleException(HttpContext context, Exception exception)
        {
            //RFC7807 - Problem Details for HTTP APIs
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.HttpContext.Request.Path,
                Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
                Title = exception.Message,
                Status = StatusCodes.Status500InternalServerError
            };

            if (_environment.IsDevelopment())
            {
                problemDetails.Detail = JsonConvert.SerializeObject(exception);

                if (exception.InnerException != null)
                {
                    problemDetails.Extensions["code"] = problemDetails.Status;
                    problemDetails.Extensions["Details"] = exception.InnerException;
                }
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status.Value;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
    }
}
