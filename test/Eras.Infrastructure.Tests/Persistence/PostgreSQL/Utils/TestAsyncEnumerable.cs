using System.Linq.Expressions;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

public class TestAsyncEnumerable<T>: EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> Enumerable) : base(Enumerable) { }

    public TestAsyncEnumerable(Expression Expression): base(Expression){}

    public IAsyncEnumerator<T> GetAsyncEnumerator(
        CancellationToken CancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}
