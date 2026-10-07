using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;

public sealed class GetMyProfileQueryHandler(IErasUsersRepository Repository)
    : IRequestHandler<GetMyProfileQuery, ErasUserDTO>
{
    private readonly IErasUsersRepository _repository = Repository;

    public async Task<ErasUserDTO> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        ErasUserDTO? profile = request.Sub != null
            ? await _repository.GetErasUserBySubAsync(request.Sub) ?? await _repository.GetErasUserByEmailAsync(request.Email)
            : await _repository.GetErasUserByEmailAsync(request.Email);

        return profile ?? throw new NotFoundException($"User {request.Email} not found.");
    }
}
