using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class AssessmentRepositoryActiveWorkTests : RepositoryTestBase
{
    private const string Me = "sub-me";
    private const string Other = "sub-other";

    private static AssessmentRepository CreateRepository(AppDbContext Context) =>
        new(Context, new Mock<ILogger<AssessmentRepository>>().Object);

    private static Assessment NewAssessment(
        string CreatedBy,
        AssessmentStatus Status,
        string? AssignedProfessional = null) => new()
    {
        CreatedBy = CreatedBy,
        Service = "Speech",
        AssignedProfessional = AssignedProfessional,
        StudentIds = [1],
        Status = Status,
    };

    private static IndividualIntervention NewIntervention(string? CreatedBy, InterventionStatus Status) => new()
    {
        DateUtc = DateTime.UtcNow,
        StudentIds = [1],
        CreatedBy = CreatedBy,
        Status = Status,
    };

    [Fact]
    public async Task CountActiveAssessmentsForUserAsync_CountsCreatedAndAssignedThatAreNotFinalized()
    {
        using AppDbContext context = CreateContext();
        context.Set<Assessment>().AddRange(
            NewAssessment(Me, AssessmentStatus.Remitted),
            NewAssessment(Me, AssessmentStatus.InProgress),
            NewAssessment(Other, AssessmentStatus.InProgress, AssignedProfessional: Me),
            NewAssessment(Me, AssessmentStatus.Finalized),
            NewAssessment(Other, AssessmentStatus.Remitted),
            NewAssessment(Other, AssessmentStatus.Finalized, AssignedProfessional: Me));
        await context.SaveChangesAsync();

        int count = await CreateRepository(context).CountActiveAssessmentsForUserAsync(Me);

        Assert.Equal(3, count);
    }

    [Fact]
    public async Task CountActiveAssessmentsForUserAsync_UserWithoutWork_ReturnsZero()
    {
        using AppDbContext context = CreateContext();
        context.Set<Assessment>().Add(NewAssessment(Other, AssessmentStatus.Remitted));
        await context.SaveChangesAsync();

        Assert.Equal(0, await CreateRepository(context).CountActiveAssessmentsForUserAsync(Me));
    }

    [Fact]
    public async Task CountActiveInterventionsForUserAsync_CountsOnlyOwnNotFinalizedOnes()
    {
        using AppDbContext context = CreateContext();
        Assessment assessment = NewAssessment(Other, AssessmentStatus.InProgress);
        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        context.Set<Intervention>().AddRange(
            NewIntervention(Me, InterventionStatus.Remitted),
            NewIntervention(Me, InterventionStatus.InProgress),
            NewIntervention(Me, InterventionStatus.Finalized),
            NewIntervention(Other, InterventionStatus.InProgress),
            NewIntervention(null, InterventionStatus.InProgress));
        await context.SaveChangesAsync();

        int count = await CreateRepository(context).CountActiveInterventionsForUserAsync(Me);

        Assert.Equal(2, count);
    }
}
