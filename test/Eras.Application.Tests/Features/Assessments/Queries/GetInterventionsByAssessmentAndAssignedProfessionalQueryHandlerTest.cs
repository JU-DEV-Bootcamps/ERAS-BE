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

    private static Assessment BuildAssessment(string? assignedProfessional) => new()
    {
        CreatedBy = "officer-sub",
        AssignedProfessional = assignedProfessional,
        Service = "workshop",
        Status = AssessmentStatus.Finalized,
        StudentIds = [1],
    };

    [Fact]
    public async Task Handle_WhenAssignedAndCreatedByProfessional_ReturnsOnlyTheirOwnInterventions()
    {
        var assessmentId = 2;
        var assessment = BuildAssessment("professional-sub");

        var own = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 1, 1),
            StudentIds = [1],
            CreatedBy = "professional-sub",
        };
        var someoneElses = new BuildInterventionForProfessionalTest
        {
            DateUtc = new DateTime(2026, 2, 1),
            StudentIds = [1],
            CreatedBy = "officer-sub",
        };

        var assessmentDto = new AssessmentDto
        {
            CreatedBy = "officer-sub",
            AssignedProfessional = "professional-sub",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [1],
            Interventions = [someoneElses, own],
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(assessmentDto);

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub"),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Same(own, result.ElementAt(0));
    }

    [Fact]
    public async Task Handle_WhenNotTheAssignedProfessional_ReturnsEmpty()
    {
        var assessmentId = 2;
        var assessment = BuildAssessment("someone-else");

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub"),
            CancellationToken.None);

        Assert.Empty(result);
        _mapper.Verify(x => x.Map(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoProfessionalAssigned_ReturnsEmpty()
    {
        var assessmentId = 2;
        var assessment = BuildAssessment(assignedProfessional: null);

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(assessmentId, "professional-sub"),
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
