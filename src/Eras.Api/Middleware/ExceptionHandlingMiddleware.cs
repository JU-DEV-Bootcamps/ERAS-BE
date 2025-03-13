using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Eras.Api.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidCastException ex)
            {
                _logger.LogError(ex, "Data deserialization error in CosmicLatteAPIService.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Error deserializing response from Cosmic Latte API",
                    details = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new
            {
                statusCode = response.StatusCode,
                message = exception.Message,
                detailed = exception.StackTrace
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

    }
}
