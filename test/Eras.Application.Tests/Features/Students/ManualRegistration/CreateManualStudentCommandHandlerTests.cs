using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Commands.CreateManualStudent;
using Eras.Application.Validation;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

public class CreateManualStudentCommandHandlerTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly Mock<IStudentCohortRepository> _studentCohorts = new();
    private readonly Mock<ICohortRepository> _cohorts = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly CreateManualStudentCommandHandler _handler;

    public CreateManualStudentCommandHandlerTests()
    {
        _cohorts.Setup(R => R.GetByIdAsync(3)).ReturnsAsync(new Cohort { Id = 3 });
        _students.Setup(R => R.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Student?)null);
        _profiles.Setup(R => R.ExistsByIdPassportNumberAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _currentUser.SetupGet(U => U.Sub).Returns("creator-sub");

        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<StudentRegistrationDto>>>()))
            .Returns((Func<Task<StudentRegistrationDto>> Work) => Work());

        _students.Setup(R => R.AddAsync(It.IsAny<Student>()))
            .ReturnsAsync((Student S) => { S.Id = 55; return S; });
        _studentCohorts.Setup(R => R.AddAsync(It.IsAny<Student>()))
            .ReturnsAsync((Student S) => S);
        _profiles.Setup(R => R.AddAsync(It.IsAny<StudentProfile>()))
            .ReturnsAsync((StudentProfile P) => P);

        _handler = new CreateManualStudentCommandHandler(
            _students.Object,
            _profiles.Object,
            _studentCohorts.Object,
            _cohorts.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            new StudentRegistrationDtoValidator());
    }

    private Task<StudentRegistrationDto> Run(StudentRegistrationDto dto) =>
        _handler.Handle(new CreateManualStudentCommand(dto), CancellationToken.None);

    [Fact]
    public async Task Handle_ValidRequest_CreatesStudentCohortLinkAndProfile()
    {
        StudentRegistrationDto result = await Run(StudentRegistrationTestData.ValidDto());

        Assert.Equal(55, result.StudentId);
        Assert.False(result.IsImported);
        Assert.Equal("ana.perez@jala.university", result.PrimaryEmail);
        Assert.Equal("AB-123456", result.IdPassportNumber);

        _students.Verify(R => R.AddAsync(It.Is<Student>(S =>
            !S.IsImported
            && S.Name == "Ana María Pérez"
            && S.Email == "ana.perez@jala.university"
            && S.Uuid.Length == 36
            && S.Audit.CreatedBy == "creator-sub"
            && S.StudentDetail != null)), Times.Once);
        _studentCohorts.Verify(R => R.AddAsync(It.Is<Student>(S => S.Id == 55 && S.CohortId == 3)), Times.Once);
        _profiles.Verify(R => R.AddAsync(It.Is<StudentProfile>(P =>
            P.StudentId == 55 && P.FirstName == "Ana" && P.IdPassportNumber == "AB-123456")), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidDto_ThrowsBadRequestAndCreatesNothing()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.FirstName = "";

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => Run(dto));

        Assert.Equal(400, error.StatusCode);
        _students.Verify(R => R.AddAsync(It.IsAny<Student>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public async Task Handle_MissingCohort_ThrowsBadRequest(int? cohortId)
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.CohortId = cohortId;

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => Run(dto));

        Assert.Equal(400, error.StatusCode);
    }

    [Fact]
    public async Task Handle_UnknownCohort_ThrowsNotFound()
    {
        StudentRegistrationDto dto = StudentRegistrationTestData.ValidDto();
        dto.CohortId = 99;
        _cohorts.Setup(R => R.GetByIdAsync(99)).ReturnsAsync((Cohort?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Run(dto));
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsConflict()
    {
        _students.Setup(R => R.GetByEmailAsync("ana.perez@jala.university"))
            .ReturnsAsync(new Student { Id = 1 });

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => Run(StudentRegistrationTestData.ValidDto()));

        Assert.Equal(409, error.StatusCode);
        _students.Verify(R => R.AddAsync(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateIdPassport_ThrowsConflict()
    {
        _profiles.Setup(R => R.ExistsByIdPassportNumberAsync("AB-123456", null)).ReturnsAsync(true);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => Run(StudentRegistrationTestData.ValidDto()));

        Assert.Equal(409, error.StatusCode);
        _students.Verify(R => R.AddAsync(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoAuthenticatedSub_FallsBackToSystemAuditUser()
    {
        _currentUser.SetupGet(U => U.Sub).Returns((string?)null);

        await Run(StudentRegistrationTestData.ValidDto());

        _students.Verify(R => R.AddAsync(It.Is<Student>(S => S.Audit.CreatedBy == "System")), Times.Once);
    }
}
