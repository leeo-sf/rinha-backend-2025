namespace RinhaBackend.Api.Application.Response;

public record PaymentProcessorHealthResponse(bool Failing, decimal MinResponseTime);