namespace RinhaBackend.Api.Contract;

public class RabbitMQQueueContract
{
    public string PaymentRequestedQueue { get; init; } = default!;
}