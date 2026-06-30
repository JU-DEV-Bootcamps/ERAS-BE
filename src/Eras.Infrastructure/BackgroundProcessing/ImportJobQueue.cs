using System.Threading.Channels;

using Eras.Application.Contracts.Infrastructure;

namespace Eras.Infrastructure.BackgroundProcessing
{
    /// <summary>
    /// Unbounded in-process queue of import job ids backed by a <see cref="Channel{T}"/>.
    /// Registered as a singleton so producers (the API) and the consumer (the background
    /// service) share the same channel instance.
    /// </summary>
    public sealed class ImportJobQueue : IImportJobQueue
    {
        private readonly Channel<int> _channel = Channel.CreateUnbounded<int>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        public async ValueTask EnqueueAsync(int ImportJobId, CancellationToken CancellationToken = default)
        {
            await _channel.Writer.WriteAsync(ImportJobId, CancellationToken);
        }

        public async ValueTask<int> DequeueAsync(CancellationToken CancellationToken)
        {
            return await _channel.Reader.ReadAsync(CancellationToken);
        }
    }
}
