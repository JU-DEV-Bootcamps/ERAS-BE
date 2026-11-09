
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.Answers.Commands.CreateAnswer;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class CreateRemissionCommandHandlerTests
{
    private readonly Mock<IMapper<AssessmentDto, Assessment>> _toDomainMapper;
    private readonly Mock<IMapper<Assessment, AssessmentDto>> _toDtoMapper;
    private readonly Mock<IValidator<Assessment>> _validator;
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICurrentUserService> _currentUser;
    private readonly Mock<IAssessmentActivityRepository> _activityRepository;
    private readonly List<AssessmentActivity> _recordedActivities = new();
    private readonly CreateRemissionCommandHandler _handler;

    public CreateRemissionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _validator = new Mock<IValidator<Assessment>>();
        _toDomainMapper = new Mock<IMapper<AssessmentDto, Assessment>>();
        _toDtoMapper = new Mock<IMapper<Assessment, AssessmentDto>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<Assessment>>>()))
            .Returns<Func<Task<Assessment>>>(F => F());
        _currentUser = new Mock<ICurrentUserService>();
        _currentUser.Setup(U => U.Sub).Returns("user-1");
        _currentUser.Setup(U => U.Name).Returns("Luis Gómez");
        _activityRepository = new Mock<IAssessmentActivityRepository>();
        _activityRepository
            .Setup(R => R.AddRangeAsync(It.IsAny<IReadOnlyCollection<AssessmentActivity>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<AssessmentActivity>, CancellationToken>((Items, _) => _recordedActivities.AddRange(Items))
            .Returns(Task.CompletedTask);
        _handler = new CreateRemissionCommandHandler(
            _toDomainMapper.Object,
            _toDtoMapper.Object,
            _validator.Object,
            _mockRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            _activityRepository.Object);
    }

    [Fact]
    public async Task HandleCreateRemissionAsync()
    {
        //ARRANGE
        List<StudentProfileDto> studentsData = new() {
            new StudentProfileDto{
                Id = 10,
                Name = "A",
                Email = "",
                AvgRiskLevel = 2.3,
            },
            new StudentProfileDto{
                Id = 2,
                Name = "B",
                Email = "bb",
                AvgRiskLevel = 4.3,
            }
        };
        var dto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [10, 2],
            CreatedBy = "me",
            Service = "",
            AssignedProfessionals = ["AB"],
            Students = studentsData,
            Comments = "...",
            Objective = "Goal",
            Diagnosis = "Nothing",
            Status = AssessmentStatus.Remitted,
        };
        var command = new CreateRemissionCommand(dto);

        var mappedEntity = new Assessment
        {
            Id = 1,
            StudentIds = [10, 2],
            CreatedBy = "me",
            Service = "",
            AssignedProfessionals = ["AB"],
            Comments = "...",
            Objective = "Goal",
            Diagnosis = "Nothing",
            Status = AssessmentStatus.Remitted,
        };
        var persistedEntity = mappedEntity;
        var expectedDto = new AssessmentDto
        {
            Id = 1,
            StudentIds = [10, 2],
            CreatedBy = "me",
            Service = "",
            AssignedProfessionals = ["AB"],
            Students = studentsData,
            Comments = "...",
            Objective = "Goal",
            Diagnosis = "Nothing",
            Status = AssessmentStatus.Remitted,
        };

        _toDomainMapper.Setup(m => m.Map(dto)).Returns(mappedEntity);
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository.Setup(r => r.AddAsync(mappedEntity)).ReturnsAsync(persistedEntity);
        _toDtoMapper.Setup(m => m.Map(persistedEntity)).Returns(expectedDto);

        //ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.CreatedBy, result.CreatedBy);
        Assert.Equal(expectedDto.Service, result.Service);
        Assert.Equal(expectedDto.Status, result.Status);


    }

    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-1")]
    public async Task HandleAsync_NewAssessmentWithComments_RecordsOnlyCreatedEventAsync()
    {
        // Arrange (GIVEN) a Student Services Officer named "Luis Gómez" creating an assessment with comments
        var dto = new AssessmentDto
        {
            StudentIds = [1],
            CreatedBy = "me",
            Service = "",
            Comments = "Family contacted",
            Status = AssessmentStatus.Remitted,
        };
        var mapped = new Assessment
        {
            Id = 5,
            StudentIds = [1],
            CreatedBy = "me",
            Service = "",
            Comments = "Family contacted",
            Status = AssessmentStatus.Remitted,
        };
        _toDomainMapper.Setup(M => M.Map(dto)).Returns(mapped);
        _validator
            .Setup(V => V.ValidateAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepository.Setup(R => R.AddAsync(mapped)).ReturnsAsync(mapped);
        _toDtoMapper.Setup(M => M.Map(mapped)).Returns(dto with { Id = 5 });

        // Act (WHEN) they create it
        await _handler.Handle(new CreateRemissionCommand(dto), CancellationToken.None);

        // Assert (THEN) one "Created" event with the user name; the initial comments record no "Comment added"
        AssessmentActivity activity = Assert.Single(_recordedActivities);
        Assert.Equal(AssessmentActivityType.Created, activity.Type);
        Assert.Equal(5, activity.AssessmentId);
        Assert.Equal("Luis Gómez", activity.ActorName);
        Assert.Equal(DateTimeKind.Utc, activity.OccurredAtUtc.Kind);
    }
}
