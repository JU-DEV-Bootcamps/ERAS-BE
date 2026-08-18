using Eras.Api.Filters;
using Eras.Error;
using Eras.Error.Bussiness;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class ErrorFilterTests
{
    private readonly Mock<ILogger<Exception>> _loggerMock;
    private readonly ErrorFilter _filter;

    public ErrorFilterTests()
    {
        _loggerMock = new Mock<ILogger<Exception>>();
        _filter = new ErrorFilter(_loggerMock.Object);
    }

    [Fact]
    public void OnActionExecuted_DoesNothing_WhenExceptionIsNull()
    {
        // Arrange
        var context = CreateActionExecutedContext();
        context.Exception = null;

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        Assert.False(context.ExceptionHandled);
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuted_Handles_IErasException()
    {
        // Arrange
        IErasException exception = new BussinessException("Invalid request");
        var context = CreateActionExecutedContext();
        context.Exception = (Exception)exception;

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(exception.FriendlyMessage, result.Value);
        Assert.Equal(exception.StatusCode, result.StatusCode);
    }

    [Fact]
    public void OnActionExecuted_Wraps_NormalException_InCriticalException()
    {
        // Arrange
        var exception = new Exception("Unexpected error");
        var context = CreateActionExecutedContext();
        context.Exception = exception;

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        Assert.True(context.ExceptionHandled);
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.StatusCode);
    }

    [Fact]
    public void OnActionExecuting_DoesNothing()
    {
        // Arrange
        var context = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        // Act
        _filter.OnActionExecuting(context);

        // Assert
        Assert.True(true);
    }

    private static ActionExecutedContext CreateActionExecutedContext()
    {
        return new ActionExecutedContext(
            new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new object());
    }
}