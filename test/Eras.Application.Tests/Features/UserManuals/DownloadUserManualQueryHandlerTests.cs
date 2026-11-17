using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UserManuals;
using Eras.Application.Features.UserManuals;
using Eras.Application.Features.UserManuals.Handlers;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using FluentAssertions;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.UserManuals;

public class DownloadUserManualQueryHandlerTests
{

    private readonly Mock<IUserManualCatalog> _catalog = new();

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-12")]
    public async Task HandleAsync_UndeclaredId_ThrowsNotFoundAsync()
    {
        // Arrange (GIVEN) no manual is declared with the identifier "does-not-exist"
        _catalog
            .Setup(C => C.OpenAsync("does-not-exist", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserManualFile?)null);
        var handler = new DownloadUserManualQueryHandler(_catalog.Object);

        // Act (WHEN) a signed-in user requests its download
        Func<Task> act = () => handler.Handle(new DownloadUserManualQuery("does-not-exist"), CancellationToken.None);

        // Assert (THEN / AND) the manual is reported as not found and no content is returned
        (await act.Should().ThrowAsync<UserManualsException>()).Which.StatusCode.Should().Be(404);
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-14")]
    public async Task HandleAsync_OdtManual_ReturnsOdtContentTypeAndDeclaredNameAsync()
    {
        // Arrange (GIVEN) a declared .odt manual whose declared file name is "admin-guide.odt"
        using var content = new MemoryStream([1, 2, 3]);
        var manual = new UserManual("admin-guide", "Admin Guide", "en", "1.0", "admin-guide.odt", UserManualFormat.Odt);
        _catalog
            .Setup(C => C.OpenAsync("admin-guide", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserManualFile(content, manual));
        var handler = new DownloadUserManualQueryHandler(_catalog.Object);

        // Act (WHEN) a signed-in user requests its download
        UserManualFileDto result = await handler.Handle(new DownloadUserManualQuery("admin-guide"), CancellationToken.None);

        // Assert (THEN / AND) the file comes with the ODT content type and the declared name
        result.Content.Should().BeSameAs(content);
        result.ContentType.Should().Be("application/vnd.oasis.opendocument.text");
        result.FileName.Should().Be("admin-guide.odt");
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-4")]
    public async Task HandleAsync_UndeclaredId_ThrowsNotFoundWithCodeAsync()
    {
        // Arrange (GIVEN) no manual is declared with the identifier "does-not-exist"
        _catalog
            .Setup(C => C.OpenAsync("does-not-exist", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserManualFile?)null);
        var handler = new DownloadUserManualQueryHandler(_catalog.Object);

        // Act (WHEN) a signed-in user requests the download of "does-not-exist"
        Func<Task> act = () => handler.Handle(new DownloadUserManualQuery("does-not-exist"), CancellationToken.None);

        // Assert (THEN / AND) the failure is a 404 with the not-found code and no content is returned
        UserManualsException error = (await act.Should().ThrowAsync<UserManualsException>()).Which;
        error.StatusCode.Should().Be(404);
        error.ErrorCode.Should().Be("manuals_not_found");
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-24")]
    public async Task HandleAsync_UndeclaredId_LogsTheRequestedIdentifierAsync()
    {
        // Arrange (GIVEN) no manual is declared with the identifier "does-not-exist"
        _catalog
            .Setup(C => C.OpenAsync("does-not-exist", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserManualFile?)null);
        var handler = new DownloadUserManualQueryHandler(_catalog.Object);

        // Act (WHEN) a signed-in user requests the download of "does-not-exist"
        Func<Task> act = () => handler.Handle(new DownloadUserManualQuery("does-not-exist"), CancellationToken.None);
        UserManualsException error = (await act.Should().ThrowAsync<UserManualsException>()).Which;
        var logged = new List<string>();
        error.OnLoggingException += (_, Message) => logged.Add(Message);
        error.LogException();

        // Assert (THEN / AND) the server log names the identifier and says no manual is declared with it
        logged.Should().ContainSingle().Which.Should().Contain("does-not-exist").And.Contain("No manual is declared");
    }

    [Fact]
    public async Task HandleAsync_IdentifierWithLineBreaks_LogsItWithoutThemAsync()
    {
        _catalog
            .Setup(C => C.OpenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserManualFile?)null);
        var handler = new DownloadUserManualQueryHandler(_catalog.Object);

        Func<Task> act = () => handler.Handle(new DownloadUserManualQuery("a" + Environment.NewLine + "FAKE"), CancellationToken.None);
        UserManualsException error = (await act.Should().ThrowAsync<UserManualsException>()).Which;
        var logged = new List<string>();
        error.OnLoggingException += (_, Message) => logged.Add(Message);
        error.LogException();

        logged.Should().ContainSingle().Which.Should().NotContainAny("\r", "\n");
    }
}
