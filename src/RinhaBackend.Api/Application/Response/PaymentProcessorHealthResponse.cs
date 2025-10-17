namespace RinhaBackend.Api.Application.Response;

public record PaymentProcessorHealthResponse(bool Failing, int MinResponseTime);