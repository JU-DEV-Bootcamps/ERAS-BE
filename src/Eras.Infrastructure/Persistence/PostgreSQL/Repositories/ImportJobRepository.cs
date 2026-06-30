using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ImportJobRepository(AppDbContext Context)
        : BaseRepository<ImportJob, ImportJobEntity>(Context, ImportJobMapper.ToDomain, ImportJobMapper.ToPersistence), IImportJobRepository
    {
    }
}
