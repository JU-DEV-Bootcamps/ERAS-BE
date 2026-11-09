using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;

public sealed class GetMyProfessionalProfileQueryHandler(
    IErasUsersRepository UsersRepository,
    IProfessionalProfileRepository ProfilesRepository)
    : IRequestHandler<GetMyProfessionalProfileQuery, ProfessionalProfileDTO>
{
    private readonly IErasUsersRepository _usersRepository = UsersRepository;
    private readonly IProfessionalProfileRepository _profilesRepository = ProfilesRepository;

    public async Task<ProfessionalProfileDTO> Handle(GetMyProfessionalProfileQuery request, CancellationToken cancellationToken)
    {
        ErasUserDTO? user = request.Sub != null
            ? await _usersRepository.GetErasUserBySubAsync(request.Sub) ?? await _usersRepository.GetErasUserByEmailAsync(request.Email)
            : await _usersRepository.GetErasUserByEmailAsync(request.Email);

        if (user?.Id is not int userId)
            throw new NotFoundException($"User {request.Email} not found.");

        ProfessionalProfile profile = await _profilesRepository.GetByErasUserIdAsync(userId)
            ?? new ProfessionalProfile { ErasUserId = userId };

        return profile.ToDTO(user.Position);
    }
}
