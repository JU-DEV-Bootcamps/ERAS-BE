namespace Eras.Application.Contracts.Services;

public interface IAttachmentRelocationService
{
    Task RunAsync(CancellationToken CancellationToken = default);
}
