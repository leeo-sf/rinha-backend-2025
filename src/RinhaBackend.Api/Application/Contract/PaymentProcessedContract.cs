using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Application.Contract;

public record PaymentProcessedContract(Guid CorrelationId, decimal Amount, DateTime RequestedAt, bool IsProcessed, ProcessedByEnum ProcessedBy);