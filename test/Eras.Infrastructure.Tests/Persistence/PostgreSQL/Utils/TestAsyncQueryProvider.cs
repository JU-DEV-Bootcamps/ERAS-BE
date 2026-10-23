
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Query;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider Inner)
    {
        _inner = Inner;
    }

    public IQueryable CreateQuery(Expression Expression) => new TestAsyncEnumerable<TEntity>(Expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression Expression) => new TestAsyncEnumerable<TElement>(Expression);

    public object? Execute(Expression Expression) => _inner.Execute(Expression);

    public TResult Execute<TResult>(Expression Expression) => _inner.Execute<TResult>(Expression);

    public TResult ExecuteAsync<TResult>(
    Expression Expression,
    CancellationToken CancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];

        var executeMethod = typeof(IQueryProvider)
            .GetMethods()
            .Single(Method =>
                Method.Name == nameof(IQueryProvider.Execute) &&
                Method.IsGenericMethod &&
                Method.GetParameters().Length == 1);

        var executionResult = executeMethod
            .MakeGenericMethod(resultType)
            .Invoke(_inner, new object[] { Expression });

        var taskResult = typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { executionResult });

        return (TResult)taskResult!;
    }

}
