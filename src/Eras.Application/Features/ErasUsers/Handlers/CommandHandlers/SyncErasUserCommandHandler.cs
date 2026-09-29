using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;

public sealed class SyncErasUserCommandHandler(
    IErasUsersRepository Repository,
    IValidator<ErasUser> Validator
) : IRequestHandler<SyncErasUserCommand, ErasUserDTO>
{
    private readonly IErasUsersRepository _repository = Repository;
    private readonly IValidator<ErasUser> _validator = Validator;

    public async Task<ErasUserDTO> Handle(
        SyncErasUserCommand request,
        CancellationToken cancellationToken
    )
    {
        ErasUserDTO? existingUser = request.Sub != null
            ? await _repository.GetErasUserBySubAsync(request.Sub) ?? await _repository.GetErasUserByEmailAsync(request.Email)
            : await _repository.GetErasUserByEmailAsync(request.Email);

        if (existingUser is null)
            return await CreateAsync(request, cancellationToken);

        // Keycloak is the source of truth for role assignment: a user starts as Guest
        // and gets a real role assigned there later, so every login must refresh it
        // rather than only syncing once.
        return await UpdateAsync(existingUser, request, cancellationToken);
    }

    private async Task<ErasUserDTO> CreateAsync(SyncErasUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new ErasUserDTO
        {
            Sub = request.Sub,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            IsSynced = true,
            Audit = new AuditInfo
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System",
                ModifiedAt = DateTime.UtcNow,
            },
        };

        ErasUser entity = dto.ToDomain();
        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        ErasUser persisted = await _repository.AddAsync(entity);
        return persisted.ToDTO();
    }

    private async Task<ErasUserDTO> UpdateAsync(
        ErasUserDTO existingUser,
        SyncErasUserCommand request,
        CancellationToken cancellationToken
    )
    {
        existingUser.IsSynced = true;
        existingUser.Sub = request.Sub;
        existingUser.Email = request.Email;
        existingUser.FirstName = request.FirstName;
        existingUser.LastName = request.LastName;
        existingUser.Role = request.Role;
        existingUser.Audit.ModifiedAt = DateTime.UtcNow;
        existingUser.Audit.ModifiedBy = "System";

        ErasUser entity = existingUser.ToDomain();
        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        ErasUser updated = await _repository.UpdateAsync(entity);
        return updated.ToDTO();
    }
}
