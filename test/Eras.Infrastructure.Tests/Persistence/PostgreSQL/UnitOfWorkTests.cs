using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL
{
    public class UnitOfWorkTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ExecuteInTransactionAsync_Generic_ReturnsWorkResult()
        {
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            var result = await unitOfWork.ExecuteInTransactionAsync(() => Task.FromResult(42));

            Assert.Equal(42, result);
        }

        [Fact]
        public async Task ExecuteInTransactionAsync_Generic_WorkThrows_PropagatesException()
        {
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                unitOfWork.ExecuteInTransactionAsync<int>(() => throw new InvalidOperationException("boom")));
        }

        [Fact]
        public async Task ExecuteInTransactionAsync_NonGeneric_ExecutesWork()
        {
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);
            var executed = false;

            await unitOfWork.ExecuteInTransactionAsync(() =>
            {
                executed = true;
                return Task.CompletedTask;
            });

            Assert.True(executed);
        }

        [Fact]
        public async Task ExecuteInTransactionAsync_NonGeneric_WorkThrows_PropagatesException()
        {
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                unitOfWork.ExecuteInTransactionAsync(() => throw new InvalidOperationException("boom")));
        }
    }
}