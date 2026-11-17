using System.Text;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;
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


    private FileSystemUserManualCatalog CreateCatalogWithManifest(params string[] Entries)
    {
        File.WriteAllText(Path.Combine(_folder, "manifest.json"), $$"""{"manuals":[{{string.Join(",", Entries)}}]}""");
        return new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));
    }

    private static string Entry(string Id, string FileName, string Format) =>
        $$"""{"id":"{{Id}}","title":"{{Id}}","language":"en","version":"1.0","fileName":"{{FileName}}","format":"{{Format}}"}""";

    private static string LoggedBy(UserManualsException Exception)
    {
        var messages = new List<string>();
        Exception.OnLoggingException += (_, Message) => messages.Add(Message);
        Exception.LogException();
        return string.Join(Environment.NewLine, messages);
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

        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        Assert.Equal("manuals_manifest_invalid", error.ErrorCode);
    }

    [Fact]
    public async Task ListAsync_ManifestIsNotJson_ThrowsManifestInvalidAsync()
    {
        File.WriteAllText(Path.Combine(_folder, "manifest.json"), "not json");
        var catalog = new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));

        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        Assert.Equal("manuals_manifest_invalid", error.ErrorCode);
    }

    [Fact]
    public async Task OpenAsync_NoManifest_ThrowsManifestMissingAsync()
    {
        var catalog = new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));

        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.OpenAsync("student-guide"));

        Assert.Equal("manuals_manifest_missing", error.ErrorCode);
    }

    [Fact]
    public async Task ListAsync_NoManifest_FriendlyMessageDoesNotNameTheFolderAsync()
    {
        var catalog = new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));

        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        Assert.DoesNotContain(_folder, error.FriendlyMessage);
        Assert.Contains(_folder, LoggedBy(error));
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-1")]
    public async Task ListAsync_NoManifest_ThrowsManifestMissingAsync()
    {
        // Arrange (GIVEN) the manuals folder has no manifest
        var catalog = new FileSystemUserManualCatalog(Options.Create(new UserManualsSettings { Path = _folder }));

        // Act (WHEN) a signed-in user requests the list of manuals
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        // Assert (THEN / AND) the failure is a 500 with the manifest-missing code and no manual is returned
        Assert.Equal("manuals_manifest_missing", error.ErrorCode);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-2")]
    public async Task ListAsync_UnknownFormat_ThrowsManifestInvalidAsync()
    {
        // Arrange (GIVEN) the manifest declares a manual "student-guide" with the unknown format "docx"
        FileSystemUserManualCatalog catalog = CreateCatalogWithManifest(Entry("student-guide", "student-guide.docx", "docx"));

        // Act (WHEN) a signed-in user requests the list of manuals
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        // Assert (THEN / AND) the failure is a 500 with the manifest-invalid code and no manual is returned
        Assert.Equal("manuals_manifest_invalid", error.ErrorCode);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-3")]
    public async Task OpenAsync_DeclaredFileMissing_ThrowsFileMissingAsync()
    {
        // Arrange (GIVEN) a declared manual whose file is not in the manuals folder
        FileSystemUserManualCatalog catalog = CreateCatalogWithManifest(
            Entry("manual-de-usuario-eras-v2-pdf", "manual-de-usuario-eras-v2.pdf", "pdf"));

        // Act (WHEN) a signed-in user requests its download
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(
            () => catalog.OpenAsync("manual-de-usuario-eras-v2-pdf"));

        // Assert (THEN / AND) the failure is a 500 with the file-missing code and no content is returned
        Assert.Equal("manuals_file_missing", error.ErrorCode);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-22")]
    public async Task OpenAsync_OneInvalidEntry_FailsEvenForAValidManualAsync()
    {
        // Arrange (GIVEN) a valid manual with its file and a manual "student-guide" with the unknown format "docx"
        File.WriteAllBytes(Path.Combine(_folder, "manual-de-usuario-eras-v2.pdf"), Encoding.ASCII.GetBytes("%PDF-1.4"));
        FileSystemUserManualCatalog catalog = CreateCatalogWithManifest(
            Entry("manual-de-usuario-eras-v2-pdf", "manual-de-usuario-eras-v2.pdf", "pdf"),
            Entry("student-guide", "student-guide.docx", "docx"));

        // Act (WHEN) a signed-in user requests the download of the valid manual
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(
            () => catalog.OpenAsync("manual-de-usuario-eras-v2-pdf"));

        // Assert (THEN / AND) the failure is a 500 with the manifest-invalid code and no content is returned
        Assert.Equal("manuals_manifest_invalid", error.ErrorCode);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-6")]
    public async Task OpenAsync_DeclaredFileMissing_LogsTheManualAndTheFileAsync()
    {
        // Arrange (GIVEN) a declared manual whose file is not in the manuals folder
        FileSystemUserManualCatalog catalog = CreateCatalogWithManifest(
            Entry("manual-de-usuario-eras-v2-pdf", "manual-de-usuario-eras-v2.pdf", "pdf"));

        // Act (WHEN) a signed-in user requests its download
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(
            () => catalog.OpenAsync("manual-de-usuario-eras-v2-pdf"));

        // Assert (THEN / AND) the server log names the manual and the file, and says the file is missing
        var logged = LoggedBy(error);
        Assert.Contains("manual-de-usuario-eras-v2-pdf", logged);
        Assert.Contains("manual-de-usuario-eras-v2.pdf", logged);
        Assert.Contains("missing", logged);
    }

    [Fact]
    [Trait("Change", "687-distinguish-user-manuals-errors")]
    [Trait("AC", "687-distinguish-user-manuals-errors/AC-7")]
    public async Task ListAsync_UnknownFormat_LogsTheManualAndTheProblemAsync()
    {
        // Arrange (GIVEN) the manifest declares a manual "student-guide" with the unknown format "docx"
        FileSystemUserManualCatalog catalog = CreateCatalogWithManifest(Entry("student-guide", "student-guide.docx", "docx"));

        // Act (WHEN) a signed-in user requests the list of manuals
        UserManualsException error = await Assert.ThrowsAsync<UserManualsException>(() => catalog.ListAsync());

        // Assert (THEN / AND) the server log names the manual and says the format "docx" is not supported
        var logged = LoggedBy(error);
        Assert.Contains("student-guide", logged);
        Assert.Contains("format 'docx' is not supported", logged);
    }
}
