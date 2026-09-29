using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Assessments.Queries;

public record BuildInterventionForCreatorTest : InterventionDto { }

public class GetInterventionsByAssessmentAndCreatorQueryHandlerTests
{
    private readonly Mock<IAssessmentRepository> _repository = new();
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _mapper = new();
    private readonly GetInterventionsByAssessmentAndCreatorQueryHandler _handler;

    public GetInterventionsByAssessmentAndCreatorQueryHandlerTests()
    {
        _handler = new GetInterventionsByAssessmentAndCreatorQueryHandler(_repository.Object, _mapper.Object);
    }

    private static Assessment BuildAssessment(string createdBy) => new()
    {
        CreatedBy = createdBy,
        Service = "workshop",
        Status = AssessmentStatus.Finalized,
        StudentIds = [1],
    };

    [Fact]
    public async Task Handle_WhenCreatorMatches_ReturnsInterventionsOrderedByDate()
    {
        var assessmentId = 2;
        var assessment = BuildAssessment("officer-sub");

        var older = new BuildInterventionForCreatorTest { DateUtc = new DateTime(2026, 1, 1), StudentIds = [1] };
        var newer = new BuildInterventionForCreatorTest { DateUtc = new DateTime(2026, 2, 1), StudentIds = [1] };

        var assessmentDto = new AssessmentDto
        {
            CreatedBy = "officer-sub",
            Service = "workshop",
            Status = AssessmentStatus.Finalized,
            StudentIds = [1],
            Interventions = [newer, older],
        };

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);
        _mapper.Setup(x => x.Map(assessment)).Returns(assessmentDto);

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndCreatorQuery(assessmentId, "officer-sub"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Same(older, result.ElementAt(0));
        Assert.Same(newer, result.ElementAt(1));
    }

    [Fact]
    public async Task Handle_WhenCreatorDoesNotMatch_ReturnsEmpty()
    {
        var assessmentId = 2;
        var assessment = BuildAssessment("someone-else");

        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync(assessment);

        var result = await _handler.Handle(
            new GetInterventionsByAssessmentAndCreatorQuery(assessmentId, "officer-sub"), CancellationToken.None);

        Assert.Empty(result);
        _mapper.Verify(x => x.Map(It.IsAny<Assessment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAssessmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        var assessmentId = 1;
        _repository.Setup(x => x.GetByIdWithInterventionsAsync(assessmentId)).ReturnsAsync((Assessment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(
                new GetInterventionsByAssessmentAndCreatorQuery(assessmentId, "officer-sub"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCreatorSubIsBlank_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(
                new GetInterventionsByAssessmentAndCreatorQuery(1, " "), CancellationToken.None));
    }
}
