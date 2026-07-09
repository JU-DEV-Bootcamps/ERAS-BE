
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
    private readonly CreateRemissionCommandHandler _handler;

    public CreateRemissionCommandHandlerTests()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _validator = new Mock<IValidator<Assessment>>();
        _toDomainMapper = new Mock<IMapper<AssessmentDto, Assessment>>();
        _toDtoMapper = new Mock<IMapper<Assessment, AssessmentDto>>();
        _handler = new CreateRemissionCommandHandler(
            _toDomainMapper.Object,
            _toDtoMapper.Object,
            _validator.Object,
            _mockRepository.Object);
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
            AssignedProfessional = "AB",
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
            AssignedProfessional = "AB",
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
            AssignedProfessional = "AB",
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

}
