namespace RinhaBackend.Api.Application.Contract;

public class RabbitMQQueueContract
{
    public string PaymentRequestedQueue { get; init; } = default!;
    public string PaymentProcessedQueue { get; init; } = default!;
}