using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class AssessmentRepositoryStudentIdsTests : RepositoryTestBase
{
    private static Assessment NewAssessment(string[] Professionals, int[] StudentIds) => new()
    {
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = "creator",
        Service = "Service",
        AssignedProfessionals = Professionals,
        StudentIds = StudentIds,
        Status = AssessmentStatus.Remitted
    };

    private static AssessmentRepository CreateRepository(AppDbContext Context) =>
        new(Context, new Mock<ILogger<AssessmentRepository>>().Object);

    [Fact]
    public async Task GetStudentIdsByAssignedProfessionalAsync_AssignedAssessments_ReturnsTheirStudentsWithoutDuplicatesAsync()
    {
        using AppDbContext context = CreateContext();
        context.Set<Assessment>().AddRange(
            NewAssessment(["ana-sub"], [1, 2]),
            NewAssessment(["ana-sub", "luis-sub"], [2, 3]),
            NewAssessment(["luis-sub"], [9]));
        await context.SaveChangesAsync();

        IReadOnlyCollection<int> result = await CreateRepository(context).GetStudentIdsByAssignedProfessionalAsync("ana-sub");

        Assert.Equal([1, 2, 3], result.OrderBy(Id => Id));
    }

    [Fact]
    public async Task GetStudentIdsByAssignedProfessionalAsync_NoAssessment_ReturnsEmptyAsync()
    {
        using AppDbContext context = CreateContext();
        context.Set<Assessment>().Add(NewAssessment(["luis-sub"], [9]));
        await context.SaveChangesAsync();

        IReadOnlyCollection<int> result = await CreateRepository(context).GetStudentIdsByAssignedProfessionalAsync("ana-sub");

        Assert.Empty(result);
    }
}
