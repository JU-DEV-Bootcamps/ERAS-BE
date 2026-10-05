using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateManualStudent;

public sealed class CreateManualStudentCommandHandler(
    IStudentRepository StudentRepository,
    IStudentProfileRepository ProfileRepository,
    IStudentCohortRepository StudentCohortRepository,
    ICohortRepository CohortRepository,
    IUnitOfWork UnitOfWork,
    ICurrentUserService CurrentUserService,
    IValidator<StudentRegistrationDto> Validator
) : IRequestHandler<CreateManualStudentCommand, StudentRegistrationDto>
{
    public async Task<StudentRegistrationDto> Handle(
        CreateManualStudentCommand Request,
        CancellationToken CancellationToken)
    {
        StudentRegistrationDto dto = Request.Profile;
        await StudentProfileGuards.ValidateAsync(Validator, dto, CancellationToken);

        if (dto.CohortId is null or <= 0)
            throw new BussinessException("Cohort is required.", 400);

        Cohort cohort = await CohortRepository.GetByIdAsync(dto.CohortId.Value)
            ?? throw new NotFoundException($"Cohort with ID {dto.CohortId} not found.");

        string email = dto.PrimaryEmail.Trim();
        if (await StudentRepository.GetByEmailAsync(email) is not null)
            throw new BussinessException("A student with this email already exists.", 409);

        string idPassport = StudentRegistrationMapper.NormalizeIdPassport(dto.IdPassportNumber);
        if (await ProfileRepository.ExistsByIdPassportNumberAsync(idPassport))
            throw new BussinessException("A student with this ID / passport number already exists.", 409);

        string actor = CurrentUserService.Sub ?? "System";

        return await UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            Student created = await StudentRepository.AddAsync(new Student
            {
                Uuid = Guid.NewGuid().ToString(),
                Name = dto.ComposeFullName(),
                Email = email,
                IsImported = false,
                StudentDetail = new StudentDetail { Audit = NewAudit(actor) },
                Audit = NewAudit(actor),
            });

            await StudentCohortRepository.AddAsync(new Student { Id = created.Id, CohortId = cohort.Id });

            StudentProfile profile = new() { StudentId = created.Id, Audit = NewAudit(actor) };
            dto.ApplyTo(profile);
            StudentProfile savedProfile = await ProfileRepository.AddAsync(profile);

            return savedProfile.ToDto(created);
        });
    }

    private static AuditInfo NewAudit(string Actor) => new()
    {
        CreatedBy = Actor,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow,
    };
}
