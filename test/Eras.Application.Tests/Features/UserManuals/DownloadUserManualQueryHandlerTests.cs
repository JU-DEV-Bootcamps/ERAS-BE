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
        await act.Should().ThrowAsync<NotFoundException>();
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
}
