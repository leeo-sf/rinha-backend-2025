using Refit;
using RinhaBackend.Api.Request;
using RinhaBackend.Api.Response;

namespace RinhaBackend.Api.Service.Api;

public interface IPaymentProcessorDefaultApi
{
    [Post("/payments")]
    Task<ApiResponse<PaymentProcessorResponse>> PaymentProcessorAsync(
        [Body] PaymentProcessorRequest request);
}