using RinhaBackend.Api.Contract.Enum;

namespace RinhaBackend.Api.Data.Entity;

public record Payment(Guid CorrelationId, decimal Amount, DateTime? RequestedAt, ProcessedByEnum? ProcessedBy, bool IsProcessed)
{
    public Guid CorrelationId { get; init; } = CorrelationId;
    public decimal Amount { get; init; } = Amount;
    public DateTime? RequestedAt { get; init; } = RequestedAt;
    public ProcessedByEnum? ProcessedBy { get; init; } = ProcessedBy;
    public bool IsProcessed { get; init; } = IsProcessed;
}