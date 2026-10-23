using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;

public sealed class GetErasUsersByRoleQueryHandler(IErasUsersRepository Repository)
    : IRequestHandler<GetErasUsersByRoleQuery, IEnumerable<ErasUserDTO>>
{
    private readonly IErasUsersRepository _repository = Repository;

    public async Task<IEnumerable<ErasUserDTO>> Handle(
        GetErasUsersByRoleQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _repository.GetErasUsersByRoleAsync(request.Role);
    }
}
