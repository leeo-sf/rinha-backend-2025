using Refit;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Response;

namespace RinhaBackend.Api.Application.Service.Api;

public interface IPaymentProcessorDefaultApi
{
    [Post("/payments")]
    Task<ApiResponse<PaymentProcessorResponse>> PaymentProcessorAsync(
        [Body] PaymentProcessorContract request);

    [Get("/payments/service-health")]
    Task<ApiResponse<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
}