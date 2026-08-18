using Eras.Application.Utils;

namespace Eras.Application.Tests.Utilities;

public class FileSignatureValidatorTest
{
    [Theory]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 }, ".pdf")] // %PDF-1.4
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 }, ".jpg")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 }, ".jpeg")]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, ".png")]
    [InlineData(new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00 }, ".docx")]
    public void IsContentValidForExtension_Should_Accept_MatchingSignature(byte[] Header, string Extension)
    {
        Assert.True(FileSignatureValidator.IsContentValidForExtension(Header, Extension));
    }

    [Theory]
    [InlineData(".pdf")]
    [InlineData(".jpg")]
    [InlineData(".png")]
    [InlineData(".docx")]
    public void IsContentValidForExtension_Should_Reject_ContentNotMatchingTheExtensionsSignature(string Extension)
    {
        byte[] plainText = "this is not the right file type at all"u8.ToArray();

        Assert.False(FileSignatureValidator.IsContentValidForExtension(plainText, Extension));
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".png")]
    [InlineData(".pdf")]
    [InlineData(".txt")]
    [InlineData(".docx")]
    public void IsContentValidForExtension_Should_Reject_WindowsExecutable_RegardlessOfClaimedExtension(string Extension)
    {
        byte[] mzHeader = [0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00];

        Assert.False(FileSignatureValidator.IsContentValidForExtension(mzHeader, Extension));
    }

    [Fact]
    public void IsContentValidForExtension_Should_Reject_ElfExecutable()
    {
        byte[] elfHeader = [0x7F, 0x45, 0x4C, 0x46, 0x02, 0x01, 0x01, 0x00];

        Assert.False(FileSignatureValidator.IsContentValidForExtension(elfHeader, ".jpg"));
    }

    [Fact]
    public void IsContentValidForExtension_Should_Accept_PlainTextContent_ForExtensionWithNoKnownSignature()
    {
        byte[] plainText = "Hello, this is a perfectly normal text file.\nSecond line."u8.ToArray();

        Assert.True(FileSignatureValidator.IsContentValidForExtension(plainText, ".txt"));
    }

    [Fact]
    public void IsContentValidForExtension_Should_Reject_ContentWithNulBytes_ForExtensionWithNoKnownSignature()
    {
        byte[] binaryJunk = [0x01, 0x00, 0x02, 0x00, 0x03, 0x00];

        Assert.False(FileSignatureValidator.IsContentValidForExtension(binaryJunk, ".txt"));
    }

    [Fact]
    public void IsContentValidForExtension_Should_Accept_EmptyHeader()
    {
        Assert.True(FileSignatureValidator.IsContentValidForExtension(ReadOnlySpan<byte>.Empty, ".txt"));
    }

    [Fact]
    public void IsContentValidForExtension_Should_BeCaseInsensitive_ForTheExtension()
    {
        byte[] pdfHeader = [0x25, 0x50, 0x44, 0x46];

        Assert.True(FileSignatureValidator.IsContentValidForExtension(pdfHeader, ".PDF"));
    }
}
