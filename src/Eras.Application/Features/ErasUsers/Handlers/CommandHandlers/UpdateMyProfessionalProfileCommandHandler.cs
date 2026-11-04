using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Application.Mappers;
using Eras.Application.Validation;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;

public sealed class UpdateMyProfessionalProfileCommandHandler(
    IErasUsersRepository UsersRepository,
    IProfessionalProfileRepository ProfilesRepository,
    IValidator<ProfessionalProfile> ProfileValidator,
    IValidator<ErasUser> UserValidator,
    IUnitOfWork UnitOfWork
) : IRequestHandler<UpdateMyProfessionalProfileCommand, ProfessionalProfileDTO>
{
    private readonly IErasUsersRepository _usersRepository = UsersRepository;
    private readonly IProfessionalProfileRepository _profilesRepository = ProfilesRepository;
    private readonly IValidator<ProfessionalProfile> _profileValidator = ProfileValidator;
    private readonly IValidator<ErasUser> _userValidator = UserValidator;
    private readonly IUnitOfWork _unitOfWork = UnitOfWork;

    public async Task<ProfessionalProfileDTO> Handle(UpdateMyProfessionalProfileCommand request, CancellationToken cancellationToken)
    {
        ErasUserDTO? userDto = request.Sub != null
            ? await _usersRepository.GetErasUserBySubAsync(request.Sub) ?? await _usersRepository.GetErasUserByEmailAsync(request.Email)
            : await _usersRepository.GetErasUserByEmailAsync(request.Email);

        if (userDto?.Id is not int userId)
            throw new NotFoundException($"User {request.Email} not found.");

        UpdateProfessionalProfileRequest input = request.Profile;

        ProfessionalProfile profile = await _profilesRepository.GetByErasUserIdAsync(userId)
            ?? new ProfessionalProfile { ErasUserId = userId };
        profile.Faculty = input.Faculty ?? string.Empty;
        profile.OfficeHours = input.OfficeHours ?? string.Empty;
        profile.SubjectsTaught = input.SubjectsTaught ?? [];
        profile.Courses = (input.Courses ?? [])
            .Select(Course => new ProfessionalCourse { Code = Course.Code, Name = Course.Name })
            .ToList();
        profile.Audit.ModifiedAt = DateTime.UtcNow;
        profile.Audit.ModifiedBy = "System";

        ErasUser user = userDto.ToDomain();
        user.Position = input.Position ?? string.Empty;
        user.Audit.ModifiedAt = DateTime.UtcNow;
        user.Audit.ModifiedBy = "System";

        // Both entities are validated before anything is written, so a rejected update changes nothing.
        try
        {
            await ValidationHelper.ValidateAndThrowAsync(_profileValidator, profile, cancellationToken);
            await ValidationHelper.ValidateAndThrowAsync(_userValidator, user, cancellationToken);
        }
        catch (ValidationException Ex)
        {
            // A validation failure is a client error: 400 with the messages that name the fields.
            throw new BussinessException(string.Join(" ", Ex.Errors.Select(Failure => Failure.ErrorMessage)), 400);
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            ProfessionalProfile saved = await _profilesRepository.SaveAsync(profile);
            await _usersRepository.UpdateAsync(user);
            return saved.ToDTO(user.Position);
        });
    }
}
