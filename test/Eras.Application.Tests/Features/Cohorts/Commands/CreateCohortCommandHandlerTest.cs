using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Cohorts.Commands.CreateCohort;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Cohorts.Commands;
public class CreateCohortCommandHandlerTest
{
    private readonly Mock<ICohortRepository> _mockCohortRepository;
    private readonly Mock<ILogger<CreateCohortCommandHandler>> _mockLogger;
    private readonly CreateCohortCommandHandler _handler;

    public CreateCohortCommandHandlerTest()
    {
        _mockCohortRepository = new Mock<ICohortRepository>();
        _mockLogger = new Mock<ILogger<CreateCohortCommandHandler>>();
        _handler = new CreateCohortCommandHandler(_mockCohortRepository.Object,_mockLogger.Object);
    }

    [Fact]
    public async Task Handler_ShouldReturnPersistedCohortIfExists()
    {
        var persistedCohort = new Cohort { Name = "Cohort_2026" };
        var newCohort = new CohortDTO { Name = "Cohort_2026" };

        var command = new CreateCohortCommand { CohortDto = newCohort };

        _mockCohortRepository.Setup(Repo => Repo.GetByNameAsync("Cohort_2026"))
            .ReturnsAsync(persistedCohort);

        CreateCommandResponse<Cohort> result = await _handler.Handle(command, CancellationToken.None);

        _mockCohortRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Cohort>()), Times.Never);

        Assert.Equal(persistedCohort, result.Entity);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handler_ShouldCreateACohort()
    {
        var newCohort = new CohortDTO { Name = "Cohort_2026" };
        Cohort createdCohort = newCohort.ToDomain();

        var command = new CreateCohortCommand { CohortDto = newCohort };
        
        _mockCohortRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Cohort>()))
            .ReturnsAsync(createdCohort);
        
        CreateCommandResponse<Cohort> result = await _handler.Handle(command, CancellationToken.None);

        _mockCohortRepository.Verify(Repo => Repo.AddAsync(It.IsAny<Cohort>()), Times.Once);

        Assert.Equal(createdCohort, result.Entity);
        Assert.Equal(1, result.SuccessfullImports);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handler_ShouldHandleExceptionAndReturnErrorResponse()
    {
        var newCohort = new CohortDTO { Name = "Error_Cohort" };

        var command = new CreateCohortCommand { CohortDto = newCohort };

        _mockCohortRepository.Setup(Repo => Repo.AddAsync(It.IsAny<Cohort>()))
            .ThrowsAsync(new Exception("Error saving cohort."));
        
        CreateCommandResponse<Cohort> result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.Entity);
        Assert.Equal(string.Empty, result.Entity.Name);
        Assert.Equal(string.Empty, result.Entity.CourseCode);
        Assert.Equal(0, result.SuccessfullImports);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);
    }
}