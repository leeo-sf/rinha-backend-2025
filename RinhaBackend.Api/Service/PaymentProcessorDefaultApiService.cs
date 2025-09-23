using RinhaBackend.Api.Exceptions;
using RinhaBackend.Api.Interface;
using RinhaBackend.Api.MediatR;
using RinhaBackend.Api.Request;
using RinhaBackend.Api.Response;
using RinhaBackend.Api.Service.Api;

namespace RinhaBackend.Api.Service;

public class PaymentProcessorDefaultApiService
    : IPaymentProcessorDefaultApiService
{
    private readonly IPaymentProcessorDefaultApi _paymentProcessorDefaultApi;

    public PaymentProcessorDefaultApiService(IPaymentProcessorDefaultApi paymentProcessorDefaultApi)
        => _paymentProcessorDefaultApi = paymentProcessorDefaultApi;

    public async Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorRequest request)
    {
        var response = await _paymentProcessorDefaultApi.PaymentProcessorAsync(request);
        return !response.IsSuccessful
            ? Result<PaymentProcessorResponse>.Fail(new PaymentProcessorException($"Payment processor default error! {response.Error.Message}"))
            : Result<PaymentProcessorResponse>.Ok(response.Content);
    }
}