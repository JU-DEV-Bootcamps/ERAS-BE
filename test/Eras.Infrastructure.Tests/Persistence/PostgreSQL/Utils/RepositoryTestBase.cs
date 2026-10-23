using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

public abstract class RepositoryTestBase
{
    protected static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
