namespace RinhaBackend.Api.MediatR.Response;

public record PaymentProcessorHealthResponse(bool Failing, decimal MinResponseTime);