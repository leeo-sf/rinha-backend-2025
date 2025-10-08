namespace RinhaBackend.Api.Application.Contract;

public record RequestsPaymentContract(Guid CorrelationId, decimal Amount);