using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;

public sealed class UpdateUserProfileCommandHandler(
    IErasUsersRepository Repository,
    IValidator<ErasUser> Validator
) : IRequestHandler<UpdateUserProfileCommand, ErasUserDTO>
{
    private readonly IErasUsersRepository _repository = Repository;
    private readonly IValidator<ErasUser> _validator = Validator;

    public async Task<ErasUserDTO> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        ErasUser entity = await _repository.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        entity.EmployeeId = request.EmployeeId;
        entity.Department = request.Department;
        entity.Phone = request.Phone;
        entity.Position = request.Position;
        entity.About = request.About;
        entity.Audit.ModifiedAt = DateTime.UtcNow;
        entity.Audit.ModifiedBy = "System";

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);
        ErasUser updated = await _repository.UpdateAsync(entity);

        return updated.ToDTO();
    }
}
