using Refit;
using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;

namespace RinhaBackend.Api.Application.Service.Api;

public interface IPaymentProcessorFallbackApi
{
    [Post("/payments")]
    Task<ApiResponse<PaymentProcessorResponse>> PaymentProcessorAsync(
        [Body] PaymentProcessorRequest request);

    [Get("/payments/service-health")]
    Task<ApiResponse<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck();
}