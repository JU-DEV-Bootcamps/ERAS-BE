using Eras.Api.Controllers;
using Eras.Application.Contracts.Infrastructure;
using Eras.Infrastructure.External.KeycloakClient;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IKeycloakAuthService<TokenResponse>> _authServiceMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IKeycloakAuthService<TokenResponse>>();
        _loggerMock = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _authServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ReturnsOk_WhenCredentialsAreValidAsync()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "password"
        };

        var token = new TokenResponse
        {
            AccessToken = "access-token",
            ExpiresIn = 2
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(request.Username, request.Password))
            .ReturnsAsync(token);

        // Act
        IActionResult result = await _controller.LoginAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(token, okResult.Value);

        _authServiceMock.Verify(
            x => x.LoginAsync(request.Username, request.Password),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUnauthorized_WhenLoginFailsAsync()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "wrong-password"
        };

        var exception = new Exception("Invalid username or password");

        _authServiceMock
            .Setup(x => x.LoginAsync(request.Username, request.Password))
            .ThrowsAsync(exception);

        // Act
        IActionResult result = await _controller.LoginAsync(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(exception.Message, unauthorizedResult.Value);

        _authServiceMock.Verify(
            x => x.LoginAsync(request.Username, request.Password),
            Times.Once);
    }
}
