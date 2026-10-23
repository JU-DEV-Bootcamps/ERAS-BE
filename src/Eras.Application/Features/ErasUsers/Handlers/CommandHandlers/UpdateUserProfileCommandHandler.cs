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
        ErasUserDTO? dto = request.Sub != null
            ? await _repository.GetErasUserBySubAsync(request.Sub) ?? await _repository.GetErasUserByEmailAsync(request.Email)
            : await _repository.GetErasUserByEmailAsync(request.Email);

        if (dto is null)
            throw new NotFoundException($"User {request.Email} not found.");

        dto.EmployeeId = request.EmployeeId;
        dto.Department = request.Department;
        dto.Phone = request.Phone;
        dto.Position = request.Position;
        dto.About = request.About;
        dto.Audit.ModifiedAt = DateTime.UtcNow;
        dto.Audit.ModifiedBy = "System";

        ErasUser entity = dto.ToDomain();
        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);
        ErasUser updated = await _repository.UpdateAsync(entity);

        return updated.ToDTO();
    }
}
