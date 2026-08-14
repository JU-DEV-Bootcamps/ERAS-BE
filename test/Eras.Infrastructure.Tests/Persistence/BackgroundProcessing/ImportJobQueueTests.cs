using Eras.Infrastructure.BackgroundProcessing;

namespace Eras.Infrastructure.Tests.Persistence.BackgroundProcessing;

public class ImportJobQueueTests
{
    [Fact]
    public async Task EnqueueAsync_ThenDequeueAsync_ReturnsEnqueuedIdAsync()
    {
        var queue = new ImportJobQueue();

        await queue.EnqueueAsync(123);

        var result = await queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(123, result);
    }

    [Fact]
    public async Task EnqueueAsync_MultipleIds_DequeueReturnsIdsInFifoOrderAsync()
    {
        var queue = new ImportJobQueue();

        await queue.EnqueueAsync(1);
        await queue.EnqueueAsync(2);
        await queue.EnqueueAsync(3);

        Assert.Equal(1, await queue.DequeueAsync(CancellationToken.None));
        Assert.Equal(2, await queue.DequeueAsync(CancellationToken.None));
        Assert.Equal(3, await queue.DequeueAsync(CancellationToken.None));
    }

    [Fact]
    public async Task DequeueAsync_WhenCancellationRequested_ThrowsOperationCanceledExceptionAsync()
    {
        var queue = new ImportJobQueue();

        using var cancellationTokenSource = new CancellationTokenSource();

        var dequeueTask = queue.DequeueAsync(
            cancellationTokenSource.Token).AsTask();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await dequeueTask);
    }

    [Fact]
    public async Task EnqueueAsync_WhenCancellationAlreadyRequested_ThrowsOperationCanceledExceptionAsync()
    {
        var queue = new ImportJobQueue();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await queue.EnqueueAsync(
                123,
                cancellationTokenSource.Token).AsTask());
    }

    [Fact]
    public async Task QueueInstances_DoNotShareItemsAsync()
    {
        var queue1 = new ImportJobQueue();
        var queue2 = new ImportJobQueue();

        await queue1.EnqueueAsync(123);

        var result = await queue1.DequeueAsync(CancellationToken.None);

        Assert.Equal(123, result);

        using var cancellationTokenSource = new CancellationTokenSource();

        var dequeueTask = queue2.DequeueAsync(
            cancellationTokenSource.Token).AsTask();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await dequeueTask);
    }
}
