using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Services;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Services;

public class StudentAccessServiceTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IAssessmentRepository> _assessments = new();
    private readonly KeycloakRoleNames _roleNames = new();

    private StudentAccessService CreateService(string? Role, string? Sub = "ana-sub")
    {
        _currentUser.SetupGet(User => User.Sub).Returns(Sub);
        _currentUser.SetupGet(User => User.Roles).Returns(Role is null ? [] : [Role]);
        _assessments
            .Setup(Repository => Repository.GetStudentIdsByAssignedProfessionalAsync("ana-sub"))
            .ReturnsAsync([1, 2]);
        return new StudentAccessService(_currentUser.Object, _roleNames, _assessments.Object);
    }

    [Theory]
    [InlineData("ERAS Administrator")]
    [InlineData("ERAS Student Services Officer")]
    public async Task GetVisibleStudentIdsAsync_AdministratorOrOfficer_IsUnrestrictedAsync(string Role)
    {
        StudentAccessService service = CreateService(Role);

        IReadOnlyCollection<int>? result = await service.GetVisibleStudentIdsAsync();

        Assert.Null(result);
        _assessments.Verify(Repository => Repository.GetStudentIdsByAssignedProfessionalAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetVisibleStudentIdsAsync_Professional_ReturnsTheStudentsOfTheirAssessmentsAsync()
    {
        StudentAccessService service = CreateService("ERAS Professional");

        IReadOnlyCollection<int>? result = await service.GetVisibleStudentIdsAsync();

        Assert.Equal([1, 2], result);
    }

    [Fact]
    public async Task GetVisibleStudentIdsAsync_Guest_Throws403Async()
    {
        StudentAccessService service = CreateService(null);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => service.GetVisibleStudentIdsAsync());

        Assert.Equal(403, error.StatusCode);
    }

    [Fact]
    public async Task GetVisibleStudentIdsAsync_ProfessionalWithoutSub_Throws403Async()
    {
        StudentAccessService service = CreateService("ERAS Professional", Sub: null);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => service.GetVisibleStudentIdsAsync());

        Assert.Equal(403, error.StatusCode);
    }

    [Fact]
    public async Task EnsureCanAccessStudentAsync_ProfessionalWithAssignedStudent_PassesAsync()
    {
        StudentAccessService service = CreateService("ERAS Professional");

        await service.EnsureCanAccessStudentAsync(2);
    }

    [Fact]
    public async Task EnsureCanAccessStudentAsync_ProfessionalWithOtherStudent_Throws403Async()
    {
        StudentAccessService service = CreateService("ERAS Professional");

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => service.EnsureCanAccessStudentAsync(9));

        Assert.Equal(403, error.StatusCode);
    }

    [Theory]
    [InlineData("ERAS Administrator")]
    [InlineData("ERAS Student Services Officer")]
    public async Task EnsureCanAccessStudentAsync_AdministratorOrOfficer_PassesWithoutLookingUpAssessmentsAsync(string Role)
    {
        StudentAccessService service = CreateService(Role);

        await service.EnsureCanAccessStudentAsync(9);

        _assessments.Verify(Repository => Repository.GetStudentIdsByAssignedProfessionalAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EnsureCanAccessStudentAsync_Guest_Throws403Async()
    {
        StudentAccessService service = CreateService(null);

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => service.EnsureCanAccessStudentAsync(1));

        Assert.Equal(403, error.StatusCode);
    }

    [Theory]
    [InlineData("ERAS Administrator", true)]
    [InlineData("ERAS Student Services Officer", false)]
    [InlineData("ERAS Professional", false)]
    public void IsAdministrator_ReflectsTheCurrentUserRole(string Role, bool Expected)
    {
        StudentAccessService service = CreateService(Role);

        Assert.Equal(Expected, service.IsAdministrator);
    }
}
