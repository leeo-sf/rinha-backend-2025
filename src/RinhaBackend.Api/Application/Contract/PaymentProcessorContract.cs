namespace RinhaBackend.Api.Application.Contract;

public record PaymentProcessorContract(Guid CorrelationId, decimal Amount, DateTime RequestedAt);