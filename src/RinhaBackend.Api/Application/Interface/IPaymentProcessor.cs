using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;

namespace RinhaBackend.Api.Application.Interface;

public interface IPaymentProcessor
{
    Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
    Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request);
}