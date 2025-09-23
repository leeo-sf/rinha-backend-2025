namespace RinhaBackend.Api.Request;

public record PaymentProcessorRequest(Guid correlationId, decimal Amount, DateTime RequestedAt);