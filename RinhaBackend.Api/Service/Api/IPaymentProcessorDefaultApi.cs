using Refit;
using RinhaBackend.Api.MediatR.Request;
using RinhaBackend.Api.MediatR.Response;

namespace RinhaBackend.Api.Service.Api;

public interface IPaymentProcessorDefaultApi
{
    [Post("/payments")]
    Task<ApiResponse<PaymentProcessorResponse>> PaymentProcessorAsync(
        [Body] PaymentProcessorRequest request);
}