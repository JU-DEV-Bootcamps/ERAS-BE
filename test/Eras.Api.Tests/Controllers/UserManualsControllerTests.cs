using Eras.Api.Controllers;
using Eras.Application.DTOs.UserManuals;
using Eras.Application.Features.UserManuals;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class UserManualsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly UserManualsController _controller;

    public UserManualsControllerTests()
    {
        _controller = new UserManualsController(_mediator.Object);
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-10")]
    public async Task DownloadUserManualAsync_PdfManual_ReturnsAttachmentWithDeclaredNameAsync()
    {
        // Arrange (GIVEN) a declared PDF manual whose declared file name is "student-guide.pdf"
        using var content = new MemoryStream([1, 2, 3]);
        _mediator
            .Setup(M => M.Send(It.Is<DownloadUserManualQuery>(Q => Q.Id == "student-guide"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserManualFileDto(content, "application/pdf", "student-guide.pdf"));

        // Act (WHEN) a signed-in user requests its download
        IActionResult result = await _controller.DownloadUserManualAsync("student-guide", CancellationToken.None);

        // Assert (THEN / AND) the API answers with the file content under the original name
        FileStreamResult file = Assert.IsType<FileStreamResult>(result);
        Assert.Same(content, file.FileStream);
        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal("student-guide.pdf", file.FileDownloadName);
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-8")]
    public void GetUserManualsAsync_RequiresAuthenticationAndNoRolePolicy()
    {
        // Arrange (GIVEN) the list action of the controller
        var action = typeof(UserManualsController).GetMethod(nameof(UserManualsController.GetUserManualsAsync))!;

        // Act (WHEN) its effective authorization is read
        var authorize = ReadAuthorization(action);

        // Assert (THEN / AND) it needs a signed-in user, any role, and cannot be called anonymously
        Assert.NotNull(authorize);
        Assert.Null(authorize!.Policy);
        Assert.Null(authorize.Roles);
        Assert.Empty(action.GetCustomAttributes(typeof(AllowAnonymousAttribute), true));
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-11")]
    public void DownloadUserManualAsync_RequiresAuthenticationAndNoRolePolicy()
    {
        // Arrange (GIVEN) the download action of the controller
        var action = typeof(UserManualsController).GetMethod(nameof(UserManualsController.DownloadUserManualAsync))!;

        // Act (WHEN) its effective authorization is read
        var authorize = ReadAuthorization(action);

        // Assert (THEN / AND) it needs a signed-in user, any role, and cannot be called anonymously
        Assert.NotNull(authorize);
        Assert.Null(authorize!.Policy);
        Assert.Null(authorize.Roles);
        Assert.Empty(action.GetCustomAttributes(typeof(AllowAnonymousAttribute), true));
    }

    private static AuthorizeAttribute? ReadAuthorization(System.Reflection.MethodInfo Action) =>
        Action.GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Concat(typeof(UserManualsController).GetCustomAttributes(typeof(AuthorizeAttribute), true))
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-15")]
    public void Controller_ExposesOnlyReadActions()
    {
        // Arrange (GIVEN) the list and download actions of the manuals controller
        var actions = typeof(UserManualsController)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

        // Act (WHEN) the HTTP methods they answer are read
        var httpMethods = actions
            .SelectMany(A => A.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute), true))
            .Cast<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>()
            .SelectMany(Attribute => Attribute.HttpMethods)
            .ToList();

        // Assert (THEN / AND) only GET exists, so POST, PUT, PATCH and DELETE are answered 405
        Assert.Equal(actions.Length, httpMethods.Count);
        Assert.All(httpMethods, Method => Assert.Equal("GET", Method));
    }
}
