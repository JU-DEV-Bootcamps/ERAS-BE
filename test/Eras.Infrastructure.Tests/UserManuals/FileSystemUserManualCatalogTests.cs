using System.Text;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Eras.Domain.Entities;
using Eras.Infrastructure.UserManuals;

using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.Tests.UserManuals;

public sealed class FileSystemUserManualCatalogTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("eras-manuals-").FullName;

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private FileSystemUserManualCatalog CreateCatalog(string FileName = "student-guide.pdf")
    {
        File.WriteAllText(
            Path.Combine(_folder, "manifest.json"),
            $$"""{"manuals":[{"id":"student-guide","title":"Student Guide","language":"en","version":"1.0","fileName":"{{FileName}}","format":"pdf"}]}""");
        File.WriteAllBytes(Path.Combine(_folder, "student-guide.pdf"), Encoding.ASCII.GetBytes("%PDF-1.4"));
        return new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));
    }

    [Fact]
    public async Task ListAsync_ValidManifest_ReturnsTheDeclaredManualsAsync()
    {
        FileSystemUserManualCatalog catalog = CreateCatalog();

        IReadOnlyCollection<UserManual> manuals = await catalog.ListAsync();

        UserManual manual = Assert.Single(manuals);
        Assert.Equal(new UserManual("student-guide", "Student Guide", "en", "1.0", "student-guide.pdf", UserManualFormat.Pdf), manual);
    }

    [Fact]
    public async Task OpenAsync_UndeclaredId_ReturnsNullAsync()
    {
        FileSystemUserManualCatalog catalog = CreateCatalog();

        Assert.Null(await catalog.OpenAsync("does-not-exist"));
    }

    [Fact]
    public async Task OpenAsync_DeclaredId_ReturnsTheFileContentAsync()
    {
        FileSystemUserManualCatalog catalog = CreateCatalog();

        UserManualFile? file = await catalog.OpenAsync("student-guide");

        Assert.NotNull(file);
        using Stream content = file!.Content;
        Assert.Equal('%', (char)content.ReadByte());
    }

    [Fact]
    public async Task ListAsync_FileNameWithPath_IsRejectedAsync()
    {
        FileSystemUserManualCatalog catalog = CreateCatalog("../secret.pdf");

        await Assert.ThrowsAsync<InvalidDataException>(() => catalog.ListAsync());
    }
}
