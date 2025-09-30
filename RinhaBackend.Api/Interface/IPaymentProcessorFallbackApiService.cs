using RinhaBackend.Api.MediatR;
using RinhaBackend.Api.MediatR.Request;
using RinhaBackend.Api.MediatR.Response;

namespace RinhaBackend.Api.Interface;

public interface IPaymentProcessorFallbackApiService
{
    Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request);
    Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
}