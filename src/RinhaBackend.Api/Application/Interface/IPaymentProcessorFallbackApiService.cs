using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;

namespace RinhaBackend.Api.Application.Interface;

public interface IPaymentProcessorFallbackApiService
{
    Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request);
    Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
}