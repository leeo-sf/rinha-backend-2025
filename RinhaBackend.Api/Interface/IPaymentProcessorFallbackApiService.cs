using RinhaBackend.Api.MediatR;
using RinhaBackend.Api.Request;
using RinhaBackend.Api.Response;

namespace RinhaBackend.Api.Interface;

public interface IPaymentProcessorFallbackApiService
{
    Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request);
}