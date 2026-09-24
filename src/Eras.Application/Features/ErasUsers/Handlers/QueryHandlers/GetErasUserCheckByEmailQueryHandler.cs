using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;

public class GetErasUserCheckByEmailQueryHandler(IErasUsersRepository Repository)
        : IRequestHandler<GetErasUserCheckByEmailQuery, ErasUserDTO?>
{
    private readonly IErasUsersRepository _repository = Repository;

    public async Task<ErasUserDTO?> Handle(
        GetErasUserCheckByEmailQuery request,
        CancellationToken cancellationToken
    )
    {
        ErasUserDTO? erasUserCheck = await _repository.GetErasUserByEmailAsync(request.Email)
            ?? throw new NotFoundException($"User with e-mail {request.Email} not found.");

        return erasUserCheck;
    }
}