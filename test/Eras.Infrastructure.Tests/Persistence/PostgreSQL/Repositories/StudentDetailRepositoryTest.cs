using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentDetailRepositoryTest
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByStudentId_ShouldReturnStudentDetail_WhenExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var studentDetail = new StudentDetailEntity
        {
            Id = 1,
            StudentId = 100
        };

        context.StudentDetails.Add(studentDetail);
        await context.SaveChangesAsync();

        var repository = new StudentDetailRepository(context);

        // Act
        var result = await repository.GetByStudentId(100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(100, result.StudentId);
    }

    [Fact]
    public async Task GetByStudentId_ShouldReturnNull_WhenStudentDetailDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.StudentDetails.Add(new StudentDetailEntity
        {
            Id = 1,
            StudentId = 100
        });

        await context.SaveChangesAsync();

        var repository = new StudentDetailRepository(context);

        // Act
        var result = await repository.GetByStudentId(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByStudentId_ShouldReturnNull_WhenDatabaseIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentDetailRepository(context);

        // Act
        var result = await repository.GetByStudentId(100);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateStudentDetail_WhenEntityExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var existingEntity = new StudentDetailEntity
        {
            Id = 1,
            StudentId = 100
        };

        context.StudentDetails.Add(existingEntity);
        await context.SaveChangesAsync();

        var repository = new StudentDetailRepository(context);

        var entityToUpdate = new StudentDetail
        {
            Id = 1,
            StudentId = 200
        };

        // Act
        var result = await repository.UpdateAsync(entityToUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(200, result.StudentId);

        var updatedEntity = await context.StudentDetails
            .FirstAsync(S => S.Id == 1);

        Assert.Equal(200, updatedEntity.StudentId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenEntityDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentDetailRepository(context);

        var entity = new StudentDetail
        {
            Id = 999,
            StudentId = 100
        };

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(
            () => repository.UpdateAsync(entity));

        // Assert
        Assert.Equal("Entity not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSameEntity_WhenUpdateSucceedsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var existingEntity = new StudentDetailEntity
        {
            Id = 1,
            StudentId = 100
        };

        context.StudentDetails.Add(existingEntity);
        await context.SaveChangesAsync();

        var repository = new StudentDetailRepository(context);

        var entityToUpdate = new StudentDetail
        {
            Id = 1,
            StudentId = 500
        };

        // Act
        var result = await repository.UpdateAsync(entityToUpdate);

        // Assert
        Assert.Same(entityToUpdate, result);
    }
}