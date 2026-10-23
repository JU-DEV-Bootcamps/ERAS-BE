using System.Net;
using System.Text.Json;

using Eras.Api.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Middleware
{
    public class ExceptionHandlerMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock = new();

        private static DefaultHttpContext CreateContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<(int StatusCode, JsonElement Body)> ReadResponse(DefaultHttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var raw = await reader.ReadToEndAsync();
            using var doc = JsonDocument.Parse(raw);
            return (context.Response.StatusCode, doc.RootElement.Clone());
        }

        [Fact]
        public async Task InvokeAsync_NoException_CallsNextAndDoesNotWriteResponse()
        {
            // Arrange
            var context = CreateContext();
            RequestDelegate next = _ => Task.CompletedTask;
            var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode); // default, nunca se tocó
            Assert.Equal(0, context.Response.Body.Length);
        }

        [Fact]
        public async Task InvokeAsync_InvalidCastException_ReturnsBadRequestWithCosmicLatteMessage()
        {
            // Arrange
            var context = CreateContext();
            RequestDelegate next = _ => throw new InvalidCastException("bad cast");
            var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(context);
            var (statusCode, body) = await ReadResponse(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
            Assert.Equal("Error deserializing response from Cosmic Latte API", body.GetProperty("message").GetString());
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Theory]
        [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized)]
        [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest)]
        public async Task InvokeAsync_KnownExceptionTypes_MapToExpectedStatusCode(Type exceptionType, HttpStatusCode expectedStatus)
        {
            // Arrange
            var context = CreateContext();
            var exception = (Exception)Activator.CreateInstance(exceptionType, "test message")!;
            RequestDelegate next = _ => throw exception;
            var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(context);
            var (statusCode, body) = await ReadResponse(context);

            // Assert
            Assert.Equal((int)expectedStatus, statusCode);
            Assert.Equal("test message", body.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_UnmappedException_ReturnsInternalServerError()
        {
            // Arrange
            var context = CreateContext();
            RequestDelegate next = _ => throw new InvalidOperationException("boom");
            var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(context);
            var (statusCode, body) = await ReadResponse(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
            Assert.Equal("boom", body.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_GenericException_LogsError()
        {
            // Arrange
            var context = CreateContext();
            var exception = new InvalidOperationException("boom");
            RequestDelegate next = _ => throw exception;
            var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}