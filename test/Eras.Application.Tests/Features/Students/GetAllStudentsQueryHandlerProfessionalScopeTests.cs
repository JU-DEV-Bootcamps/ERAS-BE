using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Students;

public class GetAllStudentsQueryHandlerProfessionalScopeTests
{

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentProfileRepository> _profiles = new();
    private readonly Mock<IStudentAccessService> _access = new();
    private readonly Mock<ILogger<GetAllStudentsQueryHandler>> _logger = new();

    public GetAllStudentsQueryHandlerProfessionalScopeTests()
    {
        _profiles
            .Setup(Repository => Repository.GetStudentIdsWithProfileAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync([]);
    }

    private GetAllStudentsQueryHandler CreateHandler() =>
        new(_students.Object, _profiles.Object, _access.Object, _logger.Object);

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-22")]
    public async Task HandleAsync_ProfessionalRequestsList_ReturnsOnlyAssignedStudentsAsync()
    {
        // Arrange (GIVEN) "Ana Pérez" is assigned to an assessment of "Ana Torres" (1) and to none of "Luis Anaya" (2)
        _access
            .Setup(Service => Service.GetVisibleStudentIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { 1 });
        List<Student> assigned = [new() { Id = 1, Name = "Ana Torres", Email = "ana@jala.test" }];
        _students
            .Setup(Repository => Repository.GetPagedAsyncWithJoins(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.Is<IReadOnlyCollection<int>?>(Ids => Ids != null && Ids.SequenceEqual(new[] { 1 }))))
            .ReturnsAsync((assigned, assigned.Count));
        GetAllStudentsQueryHandler handler = CreateHandler();

        // Act (WHEN) they request the Students list
        PagedResult<GetAllStudentsQueryResponse> result =
            await handler.Handle(new GetAllStudentsQuery(new Pagination()), CancellationToken.None);

        // Assert (THEN) the list shows "Ana Torres" and not "Luis Anaya"
        Assert.Equal(["Ana Torres"], result.Items.Select(Item => Item.Name));
        Assert.Equal(1, result.Count);
        _students.Verify(
            Repository => Repository.GetPagedAsyncWithJoins(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.Is<IReadOnlyCollection<int>?>(Ids => Ids == null)),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_AdministratorOrOfficer_ListsWithoutALimitAsync()
    {
        _access
            .Setup(Service => Service.GetVisibleStudentIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<int>?)null);
        List<Student> everyone = [new() { Id = 1, Name = "Ana Torres" }, new() { Id = 2, Name = "Luis Anaya" }];
        _students
            .Setup(Repository => Repository.GetPagedAsyncWithJoins(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), (IReadOnlyCollection<int>?)null))
            .ReturnsAsync((everyone, everyone.Count));
        GetAllStudentsQueryHandler handler = CreateHandler();

        PagedResult<GetAllStudentsQueryResponse> result =
            await handler.Handle(new GetAllStudentsQuery(new Pagination()), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task HandleAsync_SearchWithProfessional_PassesSearchAndAllowedIdsTogetherAsync()
    {
        _access
            .Setup(Service => Service.GetVisibleStudentIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { 2 });
        _students
            .Setup(Repository => Repository.GetPagedAsyncWithJoins(
                It.IsAny<int>(), It.IsAny<int>(), "anaya", It.IsAny<IReadOnlyCollection<int>?>()))
            .ReturnsAsync((new List<Student>(), 0));
        GetAllStudentsQueryHandler handler = CreateHandler();

        await handler.Handle(new GetAllStudentsQuery(new Pagination(), "anaya"), CancellationToken.None);

        _students.Verify(
            Repository => Repository.GetPagedAsyncWithJoins(
                It.IsAny<int>(), It.IsAny<int>(), "anaya", It.Is<IReadOnlyCollection<int>?>(Ids => Ids != null && Ids.Contains(2))),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserWithoutAnErasRole_Throws403InsteadOfAnEmptyListAsync()
    {
        _access
            .Setup(Service => Service.GetVisibleStudentIdsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BussinessException("You do not have access to this student.", 403));
        GetAllStudentsQueryHandler handler = CreateHandler();

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new GetAllStudentsQuery(new Pagination()), CancellationToken.None));

        Assert.Equal(403, error.StatusCode);
    }
}
