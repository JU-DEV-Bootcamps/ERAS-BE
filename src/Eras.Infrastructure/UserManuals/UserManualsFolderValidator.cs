using System.IO.Compression;
using System.Text;
using System.Text.Json;

using Eras.Application.Models;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.UserManuals;

/// <summary>
/// Checks a manuals folder the way the pull-request pipeline does: every declared manual has its file, every
/// file is declared, each file is of the declared type by its content and none exceeds the maximum size.
/// </summary>
public sealed class UserManualsFolderValidator(UserManualsSettings Settings)
{
    private const string PdfSignature = "%PDF-";
    private const string OdtMimeType = "application/vnd.oasis.opendocument.text";

    private readonly UserManualsSettings _settings = Settings;

    /// <returns>One message per problem; each names the file or the manual it is about. Empty when the folder is valid.</returns>
    public IReadOnlyList<string> Validate(string Folder)
    {
        var problems = new List<string>();

        if (!File.Exists(Path.Combine(Folder, UserManualsManifest.FileName)))
        {
            problems.Add($"{UserManualsManifest.FileName} is missing.");
            return problems;
        }

        IReadOnlyList<UserManualsManifest.Entry> entries;
        try
        {
            entries = UserManualsManifest.Read(Folder);
        }
        catch (JsonException Exception)
        {
            problems.Add($"{UserManualsManifest.FileName} is not valid JSON: {Exception.Message}");
            return problems;
        }

        var declaredFiles = new HashSet<string>(StringComparer.Ordinal);
        var ids = new HashSet<string>(StringComparer.Ordinal);

        foreach (UserManualsManifest.Entry entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Id) || !ids.Add(entry.Id))
                problems.Add($"Manual '{entry.Id}' has an empty or repeated id.");

            if (!UserManualsManifest.IsPlainFileName(entry.FileName))
            {
                problems.Add($"Manual '{entry.Id}' declares the file name '{entry.FileName}', which is not a plain file name.");
                continue;
            }

            declaredFiles.Add(entry.FileName!);

            if (!UserManualsManifest.TryParseFormat(entry.Format, out UserManualFormat format))
            {
                problems.Add($"{entry.FileName} declares the unknown format '{entry.Format}'.");
                continue;
            }

            ValidateFile(Path.Combine(Folder, entry.FileName!), entry.FileName!, format, problems);
        }

        foreach (var path in Directory.EnumerateFiles(Folder))
        {
            var name = Path.GetFileName(path);
            if (name != UserManualsManifest.FileName && !declaredFiles.Contains(name))
                problems.Add($"{name} is in the manuals folder but no manual declares it.");
        }

        return problems;
    }

    private void ValidateFile(string FilePath, string Name, UserManualFormat Format, List<string> Problems)
    {
        if (!File.Exists(FilePath))
        {
            Problems.Add($"{Name} is declared but missing from the manuals folder.");
            return;
        }

        if (new FileInfo(FilePath).Length > _settings.MaxFileSizeBytes)
        {
            Problems.Add($"{Name} is larger than the maximum of {_settings.MaxFileSizeBytes} bytes.");
            return;
        }

        if (!HasContentOf(FilePath, Format))
            Problems.Add($"{Name} content is not a valid {Format.ToString().ToUpperInvariant()} file.");
    }

    private static bool HasContentOf(string FilePath, UserManualFormat Format)
    {
        try
        {
            return Format switch
            {
                UserManualFormat.Pdf => StartsWithPdfSignature(FilePath),
                UserManualFormat.Odt => IsOpenDocumentText(FilePath),
                _ => false,
            };
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    private static bool StartsWithPdfSignature(string FilePath)
    {
        using FileStream stream = File.OpenRead(FilePath);
        var buffer = new byte[PdfSignature.Length];
        return stream.Read(buffer, 0, buffer.Length) == buffer.Length
            && Encoding.ASCII.GetString(buffer) == PdfSignature;
    }

    private static bool IsOpenDocumentText(string FilePath)
    {
        using ZipArchive archive = ZipFile.OpenRead(FilePath);
        ZipArchiveEntry? mimetype = archive.GetEntry("mimetype");
        if (mimetype is null)
            return false;

        using var reader = new StreamReader(mimetype.Open(), Encoding.ASCII);
        return reader.ReadToEnd().Trim() == OdtMimeType;
    }
}
