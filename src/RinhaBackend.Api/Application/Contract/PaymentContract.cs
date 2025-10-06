namespace RinhaBackend.Api.Application.Contract;

public record PaymentContract(Guid CorrelationId, decimal Amount);