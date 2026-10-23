using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Commands.UpdateStudentProfile;
using Eras.Application.Validation;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class UpdateStudentProfileCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly Mock<IStudentCohortRepository> _studentCohorts = new();
    private readonly Mock<ICohortRepository> _cohorts = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly UpdateStudentProfileCommandHandler _handler;

    public UpdateStudentProfileCommandHandlerTests()
    {
        _currentUser.SetupGet(U => U.Sub).Returns("editor-sub");
        _cohorts.Setup(R => R.GetByIdAsync(3)).ReturnsAsync(new Cohort { Id = 3 });
        _studentCohorts.Setup(R => R.GetCohortIdByStudentIdAsync(7)).ReturnsAsync(2);
        _students.Setup(R => R.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Student?)null);
        _profiles.Setup(R => R.ExistsByIdPassportNumberAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
        _profiles.Setup(R => R.AddAsync(It.IsAny<StudentProfile>())).ReturnsAsync((StudentProfile P) => P);
        _profiles.Setup(R => R.UpdateAsync(It.IsAny<StudentProfile>())).ReturnsAsync((StudentProfile P) => P);

        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<StudentRegistrationDto>>>()))
            .Returns((Func<Task<StudentRegistrationDto>> Work) => Work());

        _handler = new UpdateStudentProfileCommandHandler(
            _students.Object,
            _profiles.Object,
            _studentCohorts.Object,
            _cohorts.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            new StudentRegistrationDtoValidator());
    }

    private static Student ManualStudent() => new()
    {
        Id = 7,
        Name = "Old Name",
        Email = "old@jala.university",
        IsImported = false,
    };

    private Task<StudentRegistrationDto> Run(StudentRegistrationDto dto, int id = 7) =>
        _handler.Handle(new UpdateStudentProfileCommand(id, dto), CancellationToken.None);

    [Fact]
    public async Task Handle_UnknownStudent_ThrowsNotFound()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Run(StudentRegistrationTestData.ValidDto()));
    }

    [Fact]
    public async Task Handle_InvalidDto_ThrowsBadRequest()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.PrimaryEmail = "nope";

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => Run(dto));

        Assert.Equal(400, error.StatusCode);
    }

    [Fact]
    public async Task Handle_ManualStudentWithExistingProfile_UpdatesProfileAndIdentity()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync(new StudentProfile
        {
            Id = 1,
            StudentId = 7,
            FirstName = "Old",
            LastName = "Name",
            IdPassportNumber = "OLD",
            Audit = new AuditInfo { CreatedBy = "creator", CreatedAt = DateTime.UtcNow },
        });

        StudentRegistrationDto result = await Run(StudentRegistrationTestData.ValidDto());

        Assert.Equal("ana.perez@jala.university", result.PrimaryEmail);
        _profiles.Verify(R => R.UpdateAsync(It.Is<StudentProfile>(P =>
            P.FirstName == "Ana" && P.Audit.ModifiedBy == "editor-sub")), Times.Once);
        _students.Verify(R => R.UpdateIdentityAsync(
            7, "Ana María Pérez", "ana.perez@jala.university", "editor-sub"), Times.Once);
    }

    [Fact]
    public async Task Handle_StudentWithoutProfile_CreatesOne()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);

        await Run(StudentRegistrationTestData.ValidDto());

        _profiles.Verify(R => R.AddAsync(It.Is<StudentProfile>(P => P.StudentId == 7)), Times.Once);
        _profiles.Verify(R => R.UpdateAsync(It.IsAny<StudentProfile>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImportedStudent_KeepsNameAndEmailUntouched()
    {
        Student imported = new() { Id = 7, Name = "CSV Name", Email = "ana.perez@jala.university", IsImported = true };
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(imported);
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);

        StudentRegistrationDto result = await Run(StudentRegistrationTestData.ValidDto());

        Assert.True(result.IsImported);
        _students.Verify(R => R.UpdateIdentityAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ImportedStudentEmailChange_ThrowsBadRequest()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(
            new Student { Id = 7, Name = "CSV Name", Email = "csv@jala.university", IsImported = true });

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => Run(StudentRegistrationTestData.ValidDto()));

        Assert.Equal(400, error.StatusCode);
    }

    [Fact]
    public async Task Handle_EmailTakenByAnotherStudent_ThrowsConflict()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _students.Setup(R => R.GetByEmailAsync("ana.perez@jala.university"))
            .ReturnsAsync(new Student { Id = 99 });

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => Run(StudentRegistrationTestData.ValidDto()));

        Assert.Equal(409, error.StatusCode);
    }

    [Fact]
    public async Task Handle_IdPassportTakenByAnotherStudent_ThrowsConflict()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _profiles.Setup(R => R.ExistsByIdPassportNumberAsync("AB-123456", 7)).ReturnsAsync(true);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => Run(StudentRegistrationTestData.ValidDto()));

        Assert.Equal(409, error.StatusCode);
    }

    [Fact]
    public async Task Handle_CohortGiven_ChangesCohortAndReturnsIt()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);

        StudentRegistrationDto result = await Run(StudentRegistrationTestData.ValidDto());

        Assert.Equal(3, result.CohortId);
        _studentCohorts.Verify(R => R.ChangeCohortAsync(7, 3), Times.Once);
    }

    [Fact]
    public async Task Handle_CohortOmitted_KeepsCurrentCohort()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        _profiles.Setup(R => R.GetByStudentIdAsync(7)).ReturnsAsync((StudentProfile?)null);
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.CohortId = null;

        StudentRegistrationDto result = await Run(dto);

        Assert.Equal(2, result.CohortId);
        _studentCohorts.Verify(R => R.ChangeCohortAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnknownCohort_ThrowsNotFoundAndChangesNothing()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.CohortId = 99;
        _cohorts.Setup(R => R.GetByIdAsync(99)).ReturnsAsync((Cohort?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Run(dto));

        _studentCohorts.Verify(R => R.ChangeCohortAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _profiles.Verify(R => R.UpdateAsync(It.IsAny<StudentProfile>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NegativeCohort_ThrowsBadRequest()
    {
        _students.Setup(R => R.GetByIdAsync(7)).ReturnsAsync(ManualStudent());
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.CohortId = -1;

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => Run(dto));

        Assert.Equal(400, error.StatusCode);
    }
}
