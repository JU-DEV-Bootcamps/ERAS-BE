using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public record BuildInterventionForProfessionalTest : InterventionDto { }

public class GetInterventionsByAssessmentAndAssignedProfessionalQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper = new();
    private readonly GetInterventionsByAssessmentAndAssignedProfessionalQueryHandler _handler;

    public GetInterventionsByAssessmentAndAssignedProfessionalQueryHandlerTests()
    {
        _handler = new GetInterventionsByAssessmentAndAssignedProfessionalQueryHandler(_repository.Object, _mapper.Object);
    }

    private static Assessment BuildAssessment() => new()
    {
        CreatedBy = "officer-sub",
        AssignedProfessional = "professional-sub",
        Service = "workshop",
        Status = AssessmentStatus.Finalized,
        StudentIds = [1],
    };

    private static AssessmentDto BuildAssessmentDto(IReadOnlyCollection<InterventionDto> interventions) => new()
    {
        CreatedBy = "officer-sub",
        AssignedProfessional = "professional-sub",
        Service = "workshop",
        Status = AssessmentStatus.Finalized,
        StudentIds = [1],
        Interventions = interventions,
    };

    [Fact]
    public async Task Handle_WhenCreatedByMatchesSub_ReturnsIntervention()
    {
        var assessmentId = 1;
        var assessment = BuildAssessment();

        var createdByMe = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [1],
            CreatedBy = "professional-sub",
            Professional = "Someone Else",
        };
        var notMine = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 2, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Officer Name",
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(BuildAssessmentDto([createdByMe, notMine]));

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub"),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Same(createdByMe, result.ElementAt(0));
    }

    [Fact]
    public async Task Handle_WhenProfessionalNameMatches_ReturnsIntervention()
    {
        var assessmentId = 1;
        var assessment = BuildAssessment();

        var assignedToMe = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Prof Name",
        };
        var notMine = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 2, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Another Prof",
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(BuildAssessmentDto([assignedToMe, notMine]));

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub", "Prof Name"),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Same(assignedToMe, result.ElementAt(0));
    }

    [Fact]
    public async Task Handle_WhenBothConditionsMatch_ReturnsBothInterventions()
    {
        var assessmentId = 1;
        var assessment = BuildAssessment();

        var createdByMe = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [1],
            CreatedBy = "professional-sub",
            Professional = "Another Prof",
        };
        var assignedToMe = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 2, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Prof Name",
        };
        var notMine = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 3, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Another Prof",
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(BuildAssessmentDto([createdByMe, assignedToMe, notMine]));

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub", "Prof Name"),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(createdByMe, result);
        Assert.Contains(assignedToMe, result);
    }

    [Fact]
    public async Task Handle_WhenNeitherConditionMatches_ReturnsEmpty()
    {
        var assessmentId = 1;
        var assessment = BuildAssessment();

        var notMine = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
            Professional = "Another Prof",
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(BuildAssessmentDto([notMine]));

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub", "Prof Name"),
            CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        var assessmentId = 1;
        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync((Assessment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(
                new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub"),
                CancellationToken.None));
    }
}
