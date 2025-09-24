namespace RinhaBackend.Api.Data.Entity;

public record Payment(Guid CorrelationId, decimal Amount)
{
    public Guid CorrelationId { get; init; } = CorrelationId;
    public decimal Amount { get; init; } = Amount;
    public DateTime RequestedAt { get; init; } = default!;
}