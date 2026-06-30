using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    [ExcludeFromCodeCoverage]
    public sealed class UnitOfWork(AppDbContext Context) : IUnitOfWork
    {
        private readonly AppDbContext _context = Context;

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> Work)
        {
            // Already inside a transaction: participate in the ambient one, the outermost
            // caller owns commit/rollback.
            if (_context.Database.CurrentTransaction != null)
            {
                return await Work();
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    T result = await Work();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public Task ExecuteInTransactionAsync(Func<Task> Work) =>
            ExecuteInTransactionAsync(async () =>
            {
                await Work();
                return true;
            });
    }
}
