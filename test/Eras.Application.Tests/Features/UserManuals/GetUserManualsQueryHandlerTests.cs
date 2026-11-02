using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UserManuals;
using Eras.Application.Features.UserManuals;
using Eras.Application.Features.UserManuals.Handlers;
using Eras.Application.Mappers.UserManuals;
using Eras.Domain.Entities;

using FluentAssertions;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.UserManuals;

public class GetUserManualsQueryHandlerTests
{
    private readonly Mock<IUserManualCatalog> _catalog = new();

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-9")]
    public async Task HandleAsync_TwoDeclaredManuals_ReturnsThemOrderedByTitleWithDetailsAsync()
    {
        // Arrange (GIVEN) the repository declares a PDF "Student Guide" and an .odt "Admin Guide"
        _catalog
            .Setup(C => C.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new UserManual("student-guide", "Student Guide", "en", "1.0", "student-guide.pdf", UserManualFormat.Pdf),
                new UserManual("admin-guide", "Admin Guide", "es", "2.1", "admin-guide.odt", UserManualFormat.Odt),
            ]);
        var handler = new GetUserManualsQueryHandler(_catalog.Object, new UserManualMapper());

        // Act (WHEN) the list of manuals is requested
        IReadOnlyCollection<UserManualDto> result = await handler.Handle(new GetUserManualsQuery(), CancellationToken.None);

        // Assert (THEN / AND) "Admin Guide" comes first, each item carries its details and nothing else is listed
        result.Select(M => M.Title).Should().Equal("Admin Guide", "Student Guide");
        result.First().Should().BeEquivalentTo(new UserManualDto
        {
            Id = "admin-guide",
            Title = "Admin Guide",
            Language = "es",
            Version = "2.1",
            FileName = "admin-guide.odt",
            FileFormat = "ODT",
        });
        result.Last().FileFormat.Should().Be("PDF");
        result.Should().HaveCount(2);
    }
}
