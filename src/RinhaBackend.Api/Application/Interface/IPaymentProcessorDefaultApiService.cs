using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;

namespace RinhaBackend.Api.Application.Interface;

public interface IPaymentProcessorDefaultApiService
{
    Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request);
    Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
}