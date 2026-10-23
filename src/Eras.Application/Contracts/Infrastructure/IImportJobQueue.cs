namespace Eras.Application.Contracts.Infrastructure
{
    /// <summary>
    /// In-process queue of import job ids awaiting background processing.
    /// </summary>
    public interface IImportJobQueue
    {
        ValueTask EnqueueAsync(int ImportJobId, CancellationToken CancellationToken = default);
        ValueTask<int> DequeueAsync(CancellationToken CancellationToken);
    }
}
