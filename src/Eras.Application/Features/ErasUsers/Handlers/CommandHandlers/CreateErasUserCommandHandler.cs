using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.UserManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
public sealed class CreateErasUserCommandHandler(
    IErasUsersRepository Repository,
    IValidator<ErasUser> Validator
    ): IRequestHandler<CreateErasUserCommand, ErasUserDTO>
{
    private readonly IErasUsersRepository _repository = Repository;
    private readonly IValidator<ErasUser> _validator = Validator;

    public async Task<ErasUserDTO> Handle(
        CreateErasUserCommand request,
        CancellationToken cancellationToken
    )
    {
        ErasUserDTO dto = request.ErasUser;

        ErasUserDTO? existingEntity = dto.Sub != null
            ? await _repository.GetErasUserBySubAsync(dto.Sub!) ?? await _repository.GetErasUserByEmailAsync(dto.Email)
            : await _repository.GetErasUserByEmailAsync(dto.Email);

        if (existingEntity != null)
            throw new InvalidOperationException($"User ${dto.Email} already exists.");

        if (dto.Audit.CreatedAt == default)
            dto.Audit.CreatedAt = DateTime.UtcNow;

        if (string.IsNullOrEmpty(dto.Audit.CreatedBy))
            dto.Audit.CreatedBy = "System";
        
        ErasUser entity = dto.ToDomain();

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        ErasUser persisted = await _repository.AddAsync(entity);

        return persisted.ToDTO();
    }
}