using Eras.Application.DTOs.UserManuals;

using MediatR;

namespace Eras.Application.Features.UserManuals;

public sealed record GetUserManualsQuery : IRequest<IReadOnlyCollection<UserManualDto>>;

public sealed record DownloadUserManualQuery(string Id) : IRequest<UserManualFileDto>;
