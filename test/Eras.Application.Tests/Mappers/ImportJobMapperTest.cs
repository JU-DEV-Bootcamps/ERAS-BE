using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Application.Tests.Mappers;
public class ImportJobMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_ImportJobEntity_To_ImportJob()
    {
        var entity = new ImportJobEntity
        {
            EvaluationId = 1,
            Status = Domain.Entities.ImportJobStatus.Queued,
            TotalCount = 3,
            ProcessedCount = 2,
            ExtractedCount = 1,
            RetryCount = 1,
            ConfigurationId = 1,
            StartDate = DateTime.Now.ToString(),
            EndDate = DateTime.Now.ToString(),
            PollsPayload = "{}",
            CreatedAtUtc = DateTime.Now,
            UpdatedAtUtc = DateTime.Now,
            PollId = "p0lL1D"
        };

        ImportJob result = ImportJobMapper.ToDomain(entity);

        Assert.NotNull(result);
        Assert.Equal(entity.EvaluationId, result.EvaluationId);
        Assert.Equal(entity.Status, result.Status);
        Assert.Equal(entity.TotalCount, result.TotalCount);
        Assert.Equal(entity.ProcessedCount, result.ProcessedCount);
        Assert.Equal(entity.ExtractedCount, result.ExtractedCount);
        Assert.Equal(entity.RetryCount, result.RetryCount);
        Assert.Equal(entity.ConfigurationId, result.ConfigurationId);
        Assert.Equal(entity.StartDate, result.StartDate);
        Assert.Equal(entity.EndDate, result.EndDate);
        Assert.Equal(entity.PollsPayload, result.PollsPayload);
        Assert.Equal(entity.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(entity.UpdatedAtUtc, result.UpdatedAtUtc);
        Assert.Equal(entity.PollId, result.PollId);
    }

    [Fact]
    public void ToPersistence_Should_Convert_ImportJob_To_ImportJobEntity()
    {
        var job = new ImportJob
        {
            EvaluationId = 1,
            Status = Domain.Entities.ImportJobStatus.Queued,
            TotalCount = 3,
            ProcessedCount = 2,
            ExtractedCount = 1,
            RetryCount = 1,
            ConfigurationId = 1,
            StartDate = DateTime.Now.ToString(),
            EndDate = DateTime.Now.ToString(),
            PollsPayload = "{}",
            CreatedAtUtc = DateTime.Now,
            UpdatedAtUtc = DateTime.Now,
            PollId = "p0lL1D"
        };

        ImportJobEntity result = ImportJobMapper.ToPersistence(job);

        Assert.NotNull(result);
        Assert.Equal(job.EvaluationId, result.EvaluationId);
        Assert.Equal(job.Status, result.Status);
        Assert.Equal(job.TotalCount, result.TotalCount);
        Assert.Equal(job.ProcessedCount, result.ProcessedCount);
        Assert.Equal(job.ExtractedCount, result.ExtractedCount);
        Assert.Equal(job.RetryCount, result.RetryCount);
        Assert.Equal(job.ConfigurationId, result.ConfigurationId);
        Assert.Equal(job.StartDate, result.StartDate);
        Assert.Equal(job.EndDate, result.EndDate);
        Assert.Equal(job.PollsPayload, result.PollsPayload);
        Assert.Equal(job.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(job.UpdatedAtUtc, result.UpdatedAtUtc);
        Assert.Equal(job.PollId, result.PollId);
    }
}