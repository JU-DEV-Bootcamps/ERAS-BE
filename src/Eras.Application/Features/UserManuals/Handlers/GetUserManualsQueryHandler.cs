using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UserManuals;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.UserManuals.Handlers;

public sealed class GetUserManualsQueryHandler(
    IUserManualCatalog Catalog,
    IMapper<UserManual, UserManualDto> Mapper)
    : IRequestHandler<GetUserManualsQuery, IReadOnlyCollection<UserManualDto>>
{
    private readonly IUserManualCatalog _catalog = Catalog;
    private readonly IMapper<UserManual, UserManualDto> _mapper = Mapper;

    public async Task<IReadOnlyCollection<UserManualDto>> Handle(
        GetUserManualsQuery Request,
        CancellationToken CancellationToken)
    {
        IReadOnlyCollection<UserManual> manuals = await _catalog.ListAsync(CancellationToken);

        return manuals
            .Select(_mapper.Map)
            .OrderBy(Manual => Manual.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
