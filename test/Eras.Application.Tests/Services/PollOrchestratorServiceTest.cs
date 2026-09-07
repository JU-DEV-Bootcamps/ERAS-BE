using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Eras.Application.Features.Cohorts.Commands.CreateCohort;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Components.Queries.GetByNameAndPoll;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.CreateStudentCohort;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;
using Eras.Application.Features.Variables.Commands.CreatePollVariableList;
using Eras.Application.Features.Variables.Commands.CreateVariableList;
using Eras.Application.Features.Variables.Queries.GetWithNameAndPollId;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Services;

using Component = Domain.Entities.Component;
using Variable = Domain.Entities.Variable;
 public class PollOrchestratorServiceTest
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<PollOrchestratorService>> _logger;
    private readonly Mock<IEvaluationRepository> _evaluationRepository;
    private readonly Mock<IPollInstanceRepository> _pollInstanceRepository;
    private readonly PollOrchestratorService _service;
    private DateTime InitDate = DateTime.Now;
    public PollOrchestratorServiceTest()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<PollOrchestratorService>>();
        _evaluationRepository = new Mock<IEvaluationRepository>();
        _pollInstanceRepository = new Mock<IPollInstanceRepository>();
        _service = new PollOrchestratorService(
            _mediator.Object, _logger.Object, _evaluationRepository.Object, _pollInstanceRepository.Object);
    }

    [Fact]
    public async Task CreateStudentDetailAsync_WhenStudentDetailExists_ReturnsExistingDetail()
    {
        var studentDetail = new StudentDetail { Id = 10, StudentId = 1 };

        _mediator
            .Setup(x => x.Send(
                It.Is<GetStudentDetailByStudentIdQuery>(q => q.StudentId == 1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<StudentDetail?>(studentDetail, "Found", true));

        var result = await _service.CreateStudentDetailAsync(1);

        Assert.True(result.Success);
        Assert.Same(studentDetail, result.Entity);
        Assert.Equal("Found", result.Message);
    }

    [Fact]
    public async Task CreateStudentDetailAsync_WhenStudentDetailDoesNotExist_CreatesDetail()
    {
        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetStudentDetailByStudentIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<StudentDetail?>(null));

        var expected = new CreateCommandResponse<StudentDetail>(
            new StudentDetail { Id = 10, StudentId = 1 },
            1,
            "Created",
            true);

        _mediator
            .Setup(x => x.Send(
                It.Is<CreateStudentDetailCommand>(c => c.StudentDetailDto!.StudentId == 1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.CreateStudentDetailAsync(1);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task CreateAndSetStudentCohortAsync_WhenCohortCreated_CreatesStudentCohort()
    {
        var student = new StudentDTO { Id = 5 };
        var cohort = new CohortDTO();

        var createdCohort = new CreateCommandResponse<Cohort>(
            new Cohort { Id = 20 }, 1, "Created", true);

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateCohortCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCohort);

        _mediator
            .Setup(x => x.Send(
                It.Is<CreateStudentCohortCommand>(c => c.CohortId == 20 && c.StudentId == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<Student>(new Student { Id = 5 }, 1, "Created", true));

        var result = await _service.CreateAndSetStudentCohortAsync(student, cohort);

        Assert.Same(createdCohort, result);

        _mediator.Verify(
            x => x.Send(
                It.Is<CreateStudentCohortCommand>(c => c.CohortId == 20 && c.StudentId == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAndSetStudentCohortAsync_WhenCohortCreationFails_DoesNotCreateStudentCohort()
    {
        var student = new StudentDTO { Id = 5 };
        var cohort = new CohortDTO();

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateCohortCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<Cohort>(null, 0, "Error", false));

        var result = await _service.CreateAndSetStudentCohortAsync(student, cohort);

        Assert.False(result.Success);

        _mediator.Verify(
            x => x.Send(It.IsAny<CreateStudentCohortCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateStudentFromPollAsync_WhenStudentInformationIsMissing_ReturnsError()
    {
        var poll = new PollDTO
        {
            Components =
            [
                new ComponentDTO
                {
                    Variables = [new VariableDTO { Answer = null }]
                }
            ]
        };

        var result = await _service.CreateStudentFromPollAsync(poll);

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreateStudentFromPollAsync_ShouldCreateStudentDetails()
    {
        var student = new Student
        {
            Id = 1,
            Name = "Anne",
            Email = "s@mail.com"
        };

        var poll = new PollDTO
        {
            Components = [ new ComponentDTO { Variables = [new VariableDTO { Answer = 
            new AnswerDTO { Student = student.ToDto() } }] } ]
        };

        _mediator
            .Setup(x => x.Send(
                It.Is<GetStudentByEmailQuery>(q => q.studentEmail == "s@mail.com"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Student>(student, "", true));

        _mediator
            .Setup(x => x.Send(
                It.Is<GetStudentDetailByStudentIdQuery>(q => q.StudentId == 1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<StudentDetail?>(new StudentDetail(), "", true));

        var result = await _service.CreateStudentFromPollAsync(poll);

        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task CreateStudentFromPollAsync_ThrowsExceptionEmailNotFound_CreateStudent()
    {
        var dto = new StudentDTO
        {
            Id = 1,
            Name = "Anne",
            Email = "s@mail.com",
            Audit = new Domain.Common.AuditInfo()
            {
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                CreatedBy = "auto",
                ModifiedBy = "auto",
            }
        };

        var student = new Student
        {
            Id = 1,
            Name = "Anne",
            Email = "s@mail.com",
            Audit = new Domain.Common.AuditInfo()
            {
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                CreatedBy = "auto",
                ModifiedBy = "auto",
            }
        };

        var poll = new PollDTO
        {
            Components = [
                new ComponentDTO { 
                    Variables = [ new VariableDTO {
                    Answer = new AnswerDTO
                    {
                        Student = dto
                    }}
               ] }]
        };

        _mediator
            .Setup(x => x.Send(
                It.Is<GetStudentByEmailQuery>(q => q.studentEmail == "s@mail.com"),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not found"));

        _mediator
           .Setup(x => x.Send(
               It.Is<CreateStudentCommand>(q => q.StudentDTO == dto),
               It.IsAny<CancellationToken>()))
           .ReturnsAsync(new CreateCommandResponse<Student>(student, 0));

        var result = await _service.CreateStudentFromPollAsync(poll);

        Assert.False(result.Success);
        Assert.Equal(student, result.Entity);
    }

    [Fact]
    public async Task CreatePollAsync_WhenPollDoesNotExist_CreatesPoll()
    {
        var poll = new PollDTO
        {
            Name = "Poll 1"
        };

        var createdPoll = new Poll
        {
            Id = 10,
            Name = "Poll 1"
        };

        _mediator
            .Setup(x => x.Send(
                It.Is<GetPollByNameQuery>(q => q.pollName == "Poll 1"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(null, "", true, Models.Enums.QueryEnums.QueryResultStatus.NotFound));

        _mediator
            .Setup(x => x.Send(
                It.Is<CreatePollCommand>(c => c.Poll.Name == "Poll 1" && c.Poll.LastVersion == 1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<Poll>(createdPoll, 1, "Created", true));

        var result = await _service.CreatePollAsync(poll);

        Assert.True(result.Success);
        Assert.Same(createdPoll, result.Entity);
    }

    [Fact]
    public async Task CreatePollAsync_WhenPollExists_ReturnsExistingPoll()
    {
        var existingPoll = new Poll
        {
            Id = 10,
            Name = "Poll 1",
            LastVersion = 3
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(existingPoll, "Existing", true));

        var result = await _service.CreatePollAsync(
            new PollDTO { Name = "Poll 1" });

        Assert.True(result.Success);
        Assert.Same(existingPoll, result.Entity);
    }

    [Fact]
    public async Task CreatePollAsync_WhenQueryReturnsNullBody_ReturnsError()
    {
        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(null, "", true));

        var result = await _service.CreatePollAsync(new PollDTO { Name = "Poll 1" });

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreatePollAsync_WhenMediatorThrows_ReturnsError()
    {
        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Mediator error"));

        var result = await _service.CreatePollAsync(new PollDTO { Name = "Poll 1" });

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreateRelationshipsPollVariablesAsync_WhenMediatorThrows_ReturnsError()
    {
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreatePollVariableListCommand>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        var result = await _service.CreateRelationshipsPollVariablesAsync([], 10);

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreateVariablesAsync_WhenVariablesAreNull_ReturnsEmptyList()
    {
        var result = await _service.CreateVariablesAsync(null!, 1, 2);

        Assert.Empty(result);

        _mediator.Verify(
            x => x.Send(
                It.IsAny<GetVariablesWithNameAndPollIdQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateVariablesAsync_WhenCreationSucceeds_AssignsPollVariableIds()
    {
        var service = _service;

        await service.CreatePollAsync(new PollDTO { Name = "New Poll" });

        var variable = new Variable
        {
            Id = 1,
            Name = "Variable"
        };

        var pollVariable = new Variable
        {
            Id = 1,
            Name = "Variable",
            PollVariableId = 100
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetVariablesWithNameAndPollIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<List<Variable>>([], "", true));

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateVariableListCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<List<Variable>>([variable], 1, "Created", true));

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreatePollVariableListCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<List<Variable>>([pollVariable], 1, "Created", true));

        var result = await service.CreateVariablesAsync(
            [ new VariableDTO { Name = "Variable" } ], 10, 20);

        Assert.Single(result);
        Assert.Equal(100, result[0].PollVariableId);
    }

    [Fact]
    public async Task CreateComponentAsync_WhenCreationSucceeds_ReturnsComponent()
    {
        var component = new ComponentDTO { Name = "Component"  };

        var expected = new CreateCommandResponse<Component>(
            new Component { Id = 10, Name = "Component" }, 1, "Created", true);

        _mediator
            .Setup(x => x.Send(
                It.Is<CreateComponentCommand>(c => c.Component!.Name == "Component"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.CreateComponentAsync(component);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task CreateComponentAsync_WhenMediatorThrows_ReturnsError()
    {
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateComponentCommand>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        var result = await _service.CreateComponentAsync(new ComponentDTO { Name = "Component" });

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenAlreadyExists_ReturnsExistingInstance()
    {
        var student = new Student { Id = 1 };
        var existing = new PollInstance { Id = 20 };

        _pollInstanceRepository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(1, "poll", 5))
            .ReturnsAsync(true);

        _mediator
            .Setup(x => x.Send(
                It.Is<GetPollInstanceByUuidAndStudentIdQuery>(
                    q => q.StudentId == 1 &&
                            q.PollUuid == "poll" &&
                            q.EvaluationId == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<PollInstance>(existing));

        var result = await _service.CreatePollInstanceAsync(student, "poll", DateTime.UtcNow, 5);

        Assert.True(result.Success);
        Assert.Same(existing, result.Entity);
    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenDoesNotExist_CreatesInstance()
    {
        var student = new Student { Id = 1 };
        var finishedAt = DateTime.UtcNow;

        _pollInstanceRepository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(1, "poll", 5))
            .ReturnsAsync(false);

        var expected = new CreateCommandResponse<PollInstance>(
            new PollInstance { Id = 20 }, 1, "Created", true);

        _mediator
            .Setup(x => x.Send(
                It.Is<CreatePollInstanceCommand>(
                    c => c.PollInstance!.Uuid == "poll" && c.PollInstance.EvaluationId == 5 &&
                            c.PollInstance.FinishedAt == finishedAt),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.CreatePollInstanceAsync(student, "poll", finishedAt, 5);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenExceptionOccurs_ReturnsError()
    {
        _pollInstanceRepository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>()))
            .ThrowsAsync(new Exception("Repository error"));

        var result = await _service.CreatePollInstanceAsync(new Student { Id = 1 }, "poll", DateTime.UtcNow, 5);

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
    }

    [Fact]
    public async Task CreateComponentsAndVariablesAsync_WhenComponentCreationFails_SkipsComponent()
    {
        var component = new ComponentDTO
        {
            Name = "Component"
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateComponentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<Component>(null, 0, "Error", false));

        var result = await _service.CreateComponentsAndVariablesAsync([component], 10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ImportPollInstancesAsync_WhenPollCreationFails_ReturnsSuccessWithZeroInstances()
    {
        var poll = new PollDTO { Name = "Poll 1" };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(null, "", false));

        var result = await _service.ImportPollInstancesAsync([poll], 10);

        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
    }

    [Fact]
    public async Task ImportPollInstancesAsync_WhenEvaluationIsPending_UpdatesEvaluationAndCreatesRelationship()
    {
        var poll = new PollDTO { Name = "Poll 1" };

        var existingPoll = new Poll
        {
            Id = 20,
            Name = "Poll 1",
            LastVersion = 1
        };

        var evaluation = new Evaluation
        {
            Id = 10,
            Status = EvaluationConstants.EvaluationStatus.Pending.ToString()
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(existingPoll, "Found", true));

        _evaluationRepository
            .Setup(x => x.GetStatusById(10))
            .ReturnsAsync(evaluation);

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetComponentByNameAndPollIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Component>(null, "", false, QueryEnums.QueryResultStatus.NotFound));

        _mediator
            .Setup(x => x.Send(
                It.IsAny<CreateComponentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCommandResponse<Component>(null, 0, "Error", false));

        await _service.ImportPollInstancesAsync([poll], 10);

        Assert.Equal(EvaluationConstants.EvaluationStatus.Ready.ToString(), evaluation.Status);

        _evaluationRepository.Verify(
            x => x.UpdateAsync(evaluation),
            Times.Once);

        _mediator.Verify(
            x => x.Send(
                It.Is<CreateEvaluationPollCommand>(c => c.EvaluationDTO.PollId == 20),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportPollInstancesAsync_WhenEvaluationIsNotPending_DoesNotUpdateEvaluation()
    {
        var poll = new PollDTO { Name = "Poll 1" };

        var existingPoll = new Poll
        {
            Id = 20,
            Name = "Poll 1",
            LastVersion = 1
        };

        var evaluation = new Evaluation
        {
            Id = 10,
            Status = EvaluationConstants.EvaluationStatus.Ready.ToString()
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(existingPoll, "", true));

        _evaluationRepository
            .Setup(x => x.GetStatusById(10))
            .ReturnsAsync(evaluation);

        await _service.ImportPollInstancesAsync([poll], 10);

        _evaluationRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Evaluation>()),
            Times.Never);

        _mediator.Verify(
            x => x.Send(
                It.IsAny<CreateEvaluationPollCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportPollInstancesAsync_WhenExceptionOccurs_ReturnsError()
    {
        var existingPoll = new Poll
        {
            Id = 20,
            Name = "Poll 1",
            LastVersion = 1
        };
        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(existingPoll, "", true));

        _evaluationRepository
            .Setup(x => x.GetStatusById(10))
            .ThrowsAsync(new Exception("Unexpected error"));

        var result = await _service.ImportPollInstancesAsync([new PollDTO { Name = "Poll 1" }], 10);

        Assert.Null(result.Entity);
        Assert.False(result.Success);
        Assert.Contains(
            "Error during import process Unexpected error",
            result.Message);
    }

    [Fact]
    public async Task ImportPollInstancesAsync_WhenStudentSuccessfullyIsCreated()
    {
        var poll = new PollDTO { 
            Name = "Poll 1",
            Components =  new List<ComponentDTO>
            {
                new ComponentDTO {
                    Variables = new List<VariableDTO>
                    {
                        new VariableDTO
                        {
                            Answer = new AnswerDTO
                            {
                                Student = new StudentDTO { Email = "new"}
                            }
                        }
                    }
                }
            }
                
          
        };

        var existingPoll = new Poll
        {
            Id = 20,
            Name = "Poll 1",
            LastVersion = 1
        };

        var evaluation = new Evaluation
        {
            Id = 10,
            Status = EvaluationConstants.EvaluationStatus.Ready.ToString()
        };

        _mediator
            .Setup(x => x.Send(
                It.IsAny<GetPollByNameQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<Poll>(existingPoll, "", true));

        _evaluationRepository
            .Setup(x => x.GetStatusById(10))
            .ReturnsAsync(evaluation);

        await _service.ImportPollInstancesAsync([poll], 10);

    }

    private static PollDTO CreatePollWithStudent(string email)
    {
        return new PollDTO
        {
            Components = [ new ComponentDTO
                {
                    Variables = [ new VariableDTO
                        {
                            Answer = new AnswerDTO { Student = new StudentDTO { Email = email } }
                        }
                    ]
                }
            ]
        };
    }
}
