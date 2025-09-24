namespace RinhaBackend.Api.MediatR.Request;

public record PaymentProcessorRequest(Guid correlationId, decimal Amount, DateTime RequestedAt);