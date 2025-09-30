namespace RinhaBackend.Api.MediatR.Request;

public record PaymentProcessorRequest(Guid CorrelationId, decimal Amount, DateTime RequestedAt);