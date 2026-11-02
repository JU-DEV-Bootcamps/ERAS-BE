namespace Eras.Application.DTOs.UserManuals;

public sealed record UserManualFileDto(Stream Content, string ContentType, string FileName);
