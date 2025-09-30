using RinhaBackend.Api.Contract.Enum;

namespace RinhaBackend.Api.Contract;

public record PaymentProcessedContract(Guid CorrelationId, decimal Amount, DateTime RequestedAt, bool IsProcessed, ProcessedByEnum ProcessedBy);