namespace RinhaBackend.Api.Application.Interface;

public interface IChannelQueueService<T>
{
    ValueTask EnqueueAsync(T request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default);
}