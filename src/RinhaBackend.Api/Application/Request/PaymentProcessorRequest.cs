namespace RinhaBackend.Api.Application.Request;

public record PaymentProcessorRequest(Guid CorrelationId, decimal Amount, DateTime RequestedAt);