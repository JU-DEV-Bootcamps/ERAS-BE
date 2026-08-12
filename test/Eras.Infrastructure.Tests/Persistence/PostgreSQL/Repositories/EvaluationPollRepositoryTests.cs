using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class EvaluationPollRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly IEvaluationPollRepository _repository;

    public EvaluationPollRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new EvaluationPollRepository(_context);
    }

    [Fact]
    public async Task Add_Should_Save_EvaluationAsync()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Id = 1,
            Name = "Test"
        };

        // Act
        await _repository.AddAsync(evaluation);

        // Assert
        var result = await _repository.GetByIdAsync(evaluation.Id);

        Assert.NotNull(result);
        Assert.Equal(evaluation.Id, result.Id);
    }
}