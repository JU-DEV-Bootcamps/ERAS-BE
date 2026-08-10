using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;
namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;


public class ComponentRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllComponentsAsync()
    {
        await using var context = CreateContext();

        context.Components.AddRange(
            new ComponentEntity
            {
                Id = 1,
                Name = "Component 1"
            },
            new ComponentEntity
            {
                Id = 2,
                Name = "Component 2"
            });

        await context.SaveChangesAsync();

        var repository = new ComponentRepository(context);
        var result = (await repository.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, X => X.Id == 1);
        Assert.Contains(result, X => X.Id == 2);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoComponents_ReturnsEmptyCollectionAsync()
    {
        await using var context = CreateContext();

        var repository = new ComponentRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenComponentExists_ReturnsComponentAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAsync("Engagement");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Engagement", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenComponentDoesNotExist_ReturnsNullAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAsync("Does Not Exist");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_WhenComponentBelongsToPoll_ReturnsComponentAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        context.Variables.Add(
            new VariableEntity
            {
                Id = 10,
                ComponentId = 1
            });

        context.PollVariables.Add(
            new PollVariableJoin
            {
                VariableId = 10,
                PollId = 100
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAndPollIdAsync("Engagement", 100);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Engagement", result.Name);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_WhenComponentDoesNotExist_ReturnsNullAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAndPollIdAsync("Does Not Exist", 100);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_WhenComponentIsNotAssociatedWithPoll_ReturnsNullAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        context.Variables.Add(
            new VariableEntity
            {
                Id = 10,
                ComponentId = 1
            });

        context.PollVariables.Add(
            new PollVariableJoin
            {
                VariableId = 10,
                PollId = 200
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAndPollIdAsync("Engagement", 100);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_WhenComponentHasNoVariable_ReturnsNullAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAndPollIdAsync("Engagement", 100);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAndPollIdAsync_WhenVariableHasNoPollVariable_ReturnsNullAsync()
    {
        await using var context = CreateContext();

        context.Components.Add(
            new ComponentEntity
            {
                Id = 1,
                Name = "Engagement"
            });

        context.Variables.Add(
            new VariableEntity
            {
                Id = 10,
                ComponentId = 1
            });

        await context.SaveChangesAsync();
        var repository = new ComponentRepository(context);
        var result = await repository.GetByNameAndPollIdAsync("Engagement", 100);

        Assert.Null(result);
    }
}
