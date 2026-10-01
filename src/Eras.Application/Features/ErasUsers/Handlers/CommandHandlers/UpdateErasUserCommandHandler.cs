using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
public sealed class UpdateErasUserCommandHandler(
    IErasUsersRepository Repository,
    IValidator<ErasUser> Validator
)
    : IRequestHandler<UpdateErasUserCommand, ErasUserDTO>
{
    private readonly IErasUsersRepository _repository = Repository;
    private readonly IValidator<ErasUser> _validator = Validator;

    public async Task<ErasUserDTO> Handle(
        UpdateErasUserCommand request,
        CancellationToken cancellationToken
    )
    {
        ErasUserDTO dto = request.ErasUser;

        ErasUserDTO? existingEntity = dto.Sub != null
            ? await _repository.GetErasUserBySubAsync(dto.Sub!) ?? await _repository.GetErasUserByEmailAsync(dto.Email)
            : await _repository.GetErasUserByEmailAsync(dto.Email);

        if (existingEntity is null)
            throw new NotFoundException($"User {dto.Email} not found.");
        
        if (dto.Audit.ModifiedAt == default)
            dto.Audit.ModifiedAt = DateTime.UtcNow;

        if (string.IsNullOrEmpty(dto.Audit.ModifiedBy))
            dto.Audit.ModifiedBy = "System";

        ErasUser entity = dto.ToDomain();
        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);
        ErasUser updatedEntity = await _repository.UpdateAsync(entity);

        return updatedEntity.ToDTO();
    }
}