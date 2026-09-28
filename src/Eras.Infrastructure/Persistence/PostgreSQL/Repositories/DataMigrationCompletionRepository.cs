using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class DataMigrationCompletionRepository(AppDbContext Context) : IDataMigrationCompletionRepository
{
    public async Task<bool> IsCompletedAsync(string Name) =>
        await Context.Set<DataMigrationCompletion>().AnyAsync(Obj => Obj.Name == Name);

    public async Task MarkCompletedAsync(string Name)
    {
        // Guards against writing a duplicate row if this were ever somehow called twice for the
        // same migration — IsCompletedAsync is the read side of the same check callers already
        // made, so this stays correct even if a caller doesn't check first.
        if (await IsCompletedAsync(Name))
            return;

        Context.Set<DataMigrationCompletion>().Add(new DataMigrationCompletion
        {
            Name = Name,
            CompletedAt = DateTime.UtcNow
        });
        await Context.SaveChangesAsync();
    }
}
