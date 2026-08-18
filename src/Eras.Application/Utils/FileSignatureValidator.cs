namespace Eras.Application.Utils
{
    /// <summary>
    /// Magic-byte (file-signature) content validation, so a claimed extension is never the sole
    /// signal a file is what it says it is (User Story 1.5) — e.g. a renamed executable saved as
    /// `.jpg` still has an executable's byte signature, not a JPEG's.
    /// </summary>
    public static class FileSignatureValidator
    {
        /// <summary>Read at least this many header bytes before calling <see cref="IsContentValidForExtension"/>.</summary>
        public const int HeaderBytesToRead = 16;

        // Known-good signatures for the extensions this system actually allows. Matching one of
        // these is a strong positive signal; DOCX is only checked at the "is this a well-formed
        // ZIP container" level (its true OOXML signature), not full internal structure.
        private static readonly Dictionary<string, byte[][]> KnownSignatures = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = [[0x25, 0x50, 0x44, 0x46]], // %PDF
            [".jpg"] = [[0xFF, 0xD8, 0xFF]],
            [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
            [".png"] = [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]],
            [".docx"] = [[0x50, 0x4B, 0x03, 0x04]], // ZIP local file header (OOXML container)
        };

        // Never accepted, regardless of the claimed extension — known executable formats. This is
        // what actually catches "renamed executable disguised as an allowed extension": even for
        // extensions with no signature table entry (e.g. .txt), these are still rejected.
        private static readonly byte[][] BlockedSignatures =
        [
            [0x4D, 0x5A],                   // MZ — Windows PE/DOS executable
            [0x7F, 0x45, 0x4C, 0x46],       // ELF — Linux executable
            [0xCF, 0xFA, 0xED, 0xFE],       // Mach-O (macOS, 64-bit little-endian)
        ];

        /// <returns>
        /// <see langword="false"/> if <paramref name="header"/> matches a known-dangerous
        /// signature, or claims an extension with a known signature but doesn't match it.
        /// <see langword="true"/> if it matches the extension's known signature, or the extension
        /// has no known signature (e.g. `.txt`) and the content passes a binary-content heuristic.
        /// </returns>
        public static bool IsContentValidForExtension(ReadOnlySpan<byte> header, string extension)
        {
            if (StartsWithAny(header, BlockedSignatures))
                return false;

            if (KnownSignatures.TryGetValue(extension, out byte[][]? validSignatures))
                return StartsWithAny(header, validSignatures);

            // No fixed signature exists for this extension (plain text has none) — fall back to a
            // "does this look like binary junk" heuristic instead of trusting the extension alone.
            return LooksLikeText(header);
        }

        private static bool StartsWithAny(ReadOnlySpan<byte> header, byte[][] signatures)
        {
            foreach (byte[] signature in signatures)
            {
                if (header.Length >= signature.Length && header[..signature.Length].SequenceEqual(signature))
                    return true;
            }
            return false;
        }

        private static bool LooksLikeText(ReadOnlySpan<byte> header)
        {
            if (header.IsEmpty)
                return true;

            int suspiciousBytes = 0;
            foreach (byte b in header)
            {
                // A NUL byte is the strongest single signal of binary content.
                if (b == 0x00)
                    return false;

                bool isPrintableOrCommonWhitespace = b >= 0x20 || b is 0x09 or 0x0A or 0x0D;
                if (!isPrintableOrCommonWhitespace)
                    suspiciousBytes++;
            }

            // Tolerate a small fraction of control/high bytes (e.g. UTF-8 multi-byte sequences),
            // but a header dominated by them is very unlikely to be genuine text.
            return suspiciousBytes <= header.Length / 4;
        }
    }
}
