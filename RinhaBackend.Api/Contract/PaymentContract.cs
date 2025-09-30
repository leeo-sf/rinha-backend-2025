namespace RinhaBackend.Api.Contract;

public record PaymentContract(Guid CorrelationId, decimal Amount);