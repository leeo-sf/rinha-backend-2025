using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Domain.Entity;

public record Payment(Guid CorrelationId, decimal Amount, DateTime RequestedAt, PaymentProcessorEnum ProcessedBy)
{
    public Guid CorrelationId { get; init; } = CorrelationId;
    public decimal Amount { get; init; } = Amount;
    public DateTime RequestedAt { get; init; } = RequestedAt;
    public PaymentProcessorEnum ProcessedBy { get; init; } = ProcessedBy;
}