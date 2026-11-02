using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UserManuals;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.UserManuals.Handlers;

public sealed class DownloadUserManualQueryHandler(IUserManualCatalog Catalog)
    : IRequestHandler<DownloadUserManualQuery, UserManualFileDto>
{
    private readonly IUserManualCatalog _catalog = Catalog;

    public async Task<UserManualFileDto> Handle(
        DownloadUserManualQuery Request,
        CancellationToken CancellationToken)
    {
        UserManualFile file = await _catalog.OpenAsync(Request.Id, CancellationToken)
            ?? throw new NotFoundException($"User manual '{Request.Id}' was not found.");

        return new UserManualFileDto(file.Content, ContentTypeOf(file.Manual.Format), file.Manual.FileName);
    }

    private static string ContentTypeOf(UserManualFormat Format) => Format switch
    {
        UserManualFormat.Pdf => "application/pdf",
        UserManualFormat.Odt => "application/vnd.oasis.opendocument.text",
        _ => "application/octet-stream",
    };
}
