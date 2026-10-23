using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.Students.Commands.UpdateStudentProfile;

public sealed class UpdateStudentProfileCommandHandler(
    IStudentRepository StudentRepository,
    IStudentProfileRepository ProfileRepository,
    IUnitOfWork UnitOfWork,
    ICurrentUserService CurrentUserService,
    IValidator<StudentRegistrationDto> Validator
) : IRequestHandler<UpdateStudentProfileCommand, StudentRegistrationDto>
{
    public async Task<StudentRegistrationDto> Handle(
        UpdateStudentProfileCommand Request,
        CancellationToken CancellationToken)
    {
        StudentRegistrationDto dto = Request.Profile;
        await StudentProfileGuards.ValidateAsync(Validator, dto, CancellationToken);

        Student student = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");

        string email = dto.PrimaryEmail.Trim();
        bool emailChanged = !string.Equals(email, student.Email, StringComparison.Ordinal);

        if (emailChanged)
        {
            if (student.IsImported)
                throw new BussinessException("The email of an imported student cannot be changed.", 400);

            Student? sameEmail = await StudentRepository.GetByEmailAsync(email);
            if (sameEmail is not null && sameEmail.Id != student.Id)
                throw new BussinessException("A student with this email already exists.", 409);
        }

        string idPassport = StudentRegistrationMapper.NormalizeIdPassport(dto.IdPassportNumber);
        if (await ProfileRepository.ExistsByIdPassportNumberAsync(idPassport, student.Id))
            throw new BussinessException("A student with this ID / passport number already exists.", 409);

        string actor = CurrentUserService.Sub ?? "System";

        return await UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            StudentProfile? existing = await ProfileRepository.GetByStudentIdAsync(student.Id);
            StudentProfile saved;

            if (existing is null)
            {
                StudentProfile created = new()
                {
                    StudentId = student.Id,
                    Audit = new AuditInfo
                    {
                        CreatedBy = actor,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    },
                };
                dto.ApplyTo(created);
                saved = await ProfileRepository.AddAsync(created);
            }
            else
            {
                dto.ApplyTo(existing);
                existing.Audit.ModifiedBy = actor;
                existing.Audit.ModifiedAt = DateTime.UtcNow;
                saved = await ProfileRepository.UpdateAsync(existing);
            }

            if (!student.IsImported)
            {
                await StudentRepository.UpdateIdentityAsync(student.Id, dto.ComposeFullName(), email, actor);
                student.Name = dto.ComposeFullName();
                student.Email = email;
            }

            return saved.ToDto(student);
        });
    }
}
