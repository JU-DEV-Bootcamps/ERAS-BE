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

        public ExceptionHandlerMiddleware(RequestDelegate Next, ILogger<ExceptionHandlerMiddleware> Logger)
        {
            _next = Next;
            _logger = Logger;
        }

        public async Task InvokeAsync(HttpContext Context)
        {
            try
            {
                await _next(Context);
            }
            catch (InvalidCastException ex)
            {
                _logger.LogError(ex, "Data deserialization error in CosmicLatteAPIService.");

                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    message = "Error deserializing response from Cosmic Latte API",
                    detailed = ex.StackTrace
                };

                await Context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(Context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext Context, Exception Exception)
        {
            _logger.LogError(Exception, "Unhandled exception occurred.");

            var response = Context.Response;
            response.ContentType = "application/json";
            response.StatusCode = Exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new
            {
                statusCode = response.StatusCode,
                message = Exception.Message,
                detailed = Exception.StackTrace
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

    }
}
