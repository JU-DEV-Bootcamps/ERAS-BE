using System.Text;

using Eras.Application.Models;
using Eras.Infrastructure.UserManuals;

namespace Eras.Infrastructure.Tests.UserManuals;

public sealed class UserManualsFolderValidatorTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("eras-manuals-").FullName;
    private readonly UserManualsFolderValidator _validator = new(new UserManualsSettings());

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private void WriteManifest(params (string Id, string FileName, string Format)[] Manuals)
    {
        var entries = Manuals.Select(M =>
            $$"""{"id":"{{M.Id}}","title":"{{M.Id}}","language":"en","version":"1.0","fileName":"{{M.FileName}}","format":"{{M.Format}}"}""");
        File.WriteAllText(Path.Combine(_folder, "manifest.json"), $$"""{"manuals":[{{string.Join(",", entries)}}]}""");
    }

    private void WriteFile(string FileName, byte[] Content) => File.WriteAllBytes(Path.Combine(_folder, FileName), Content);

    private static byte[] Pdf() => Encoding.ASCII.GetBytes("%PDF-1.4\n%%EOF");

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-17")]
    public void Validate_DeclaredFileMissing_FailsNamingTheFile()
    {
        // Arrange (GIVEN) a manual declared with the file name "student-guide.pdf" and no such file in the folder
        WriteManifest(("student-guide", "student-guide.pdf", "pdf"));

        // Act (WHEN) the manuals validation runs
        IReadOnlyList<string> problems = _validator.Validate(_folder);

        // Assert (THEN) the validation fails and names "student-guide.pdf"
        Assert.Contains(problems, P => P.Contains("student-guide.pdf"));
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-21")]
    public void Validate_FileNotDeclared_FailsNamingTheFile()
    {
        // Arrange (GIVEN) a file "extra.pdf" in the folder that no manual declares
        WriteManifest(("student-guide", "student-guide.pdf", "pdf"));
        WriteFile("student-guide.pdf", Pdf());
        WriteFile("extra.pdf", Pdf());

        // Act (WHEN) the manuals validation runs
        IReadOnlyList<string> problems = _validator.Validate(_folder);

        // Assert (THEN) the validation fails and names "extra.pdf"
        Assert.Contains(problems, P => P.Contains("extra.pdf"));
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-18")]
    public void Validate_PdfWithWrongContent_FailsNamingTheFile()
    {
        // Arrange (GIVEN) a manual declared as PDF whose file content is not a PDF, with a correct extension
        WriteManifest(("student-guide", "student-guide.pdf", "pdf"));
        WriteFile("student-guide.pdf", Encoding.ASCII.GetBytes("this is plain text, not a pdf"));

        // Act (WHEN) the manuals validation runs
        IReadOnlyList<string> problems = _validator.Validate(_folder);

        // Assert (THEN / AND) the validation fails and names the file, the extension alone does not make it pass
        Assert.Contains(problems, P => P.Contains("student-guide.pdf"));
    }

    [Fact]
    [Trait("Change", "640-add-user-manuals-page")]
    [Trait("AC", "640-add-user-manuals-page/AC-19")]
    public void Validate_FileOverMaxSize_FailsNamingTheFile()
    {
        // Arrange (GIVEN) a declared manual whose file is larger than the maximum size of 10 MB
        WriteManifest(("student-guide", "student-guide.pdf", "pdf"));
        using (var stream = File.Create(Path.Combine(_folder, "student-guide.pdf")))
        {
            stream.Write(Pdf());
            stream.SetLength(10L * 1024 * 1024 + 1);
        }

        // Act (WHEN) the manuals validation runs
        IReadOnlyList<string> problems = _validator.Validate(_folder);

        // Assert (THEN) the validation fails and names the file
        Assert.Contains(problems, P => P.Contains("student-guide.pdf"));
    }

    [Fact]
    public void Validate_ValidPdfAndOdt_HasNoProblems()
    {
        WriteManifest(("student-guide", "student-guide.pdf", "pdf"), ("admin-guide", "admin-guide.odt", "odt"));
        WriteFile("student-guide.pdf", Pdf());
        WriteOdt("admin-guide.odt", "application/vnd.oasis.opendocument.text");

        Assert.Empty(_validator.Validate(_folder));
    }

    [Fact]
    public void Validate_OdtWithWrongMimeType_FailsNamingTheFile()
    {
        WriteManifest(("admin-guide", "admin-guide.odt", "odt"));
        WriteOdt("admin-guide.odt", "application/zip");

        Assert.Contains(_validator.Validate(_folder), P => P.Contains("admin-guide.odt"));
    }

    [Fact]
    public void Validate_ManifestFileNameWithPath_Fails()
    {
        WriteManifest(("secret", "../secret.pdf", "pdf"));

        Assert.Contains(_validator.Validate(_folder), P => P.Contains("../secret.pdf"));
    }

    [Fact]
    public void Validate_RealManualsFolder_HasNoProblems()
    {
        var folder = ParentRepository.ManualsFolder();
        if (folder is null || !Directory.Exists(folder))
            return; // the manuals folder is only available in a full checkout

        Assert.Empty(_validator.Validate(folder));
    }

    private void WriteOdt(string FileName, string MimeType)
    {
        using var stream = File.Create(Path.Combine(_folder, FileName));
        using var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create);
        using var writer = new StreamWriter(archive.CreateEntry("mimetype").Open(), Encoding.ASCII);
        writer.Write(MimeType);
    }
}
