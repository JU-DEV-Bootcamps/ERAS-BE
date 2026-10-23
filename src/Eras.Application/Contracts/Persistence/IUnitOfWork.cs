namespace Eras.Application.Contracts.Persistence
{
    /// <summary>
    /// Coordinates a single database transaction across multiple repository/handler calls.
    /// If a transaction is already active it is reused (the inner call participates in it),
    /// so nested calls do not open conflicting transactions.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> Work);
        Task ExecuteInTransactionAsync(Func<Task> Work);
    }
}
