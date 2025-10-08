using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Exceptions;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Application.Service.Api;

namespace RinhaBackend.Api.Application.Service;

public class DefaultPaymentProcessor
    : IPaymentProcessor
{
    private readonly IPaymentProcessorDefaultApi _paymentProcessorDefaultApi;

    public DefaultPaymentProcessor(IPaymentProcessorDefaultApi paymentProcessorDefaultApi)
        => _paymentProcessorDefaultApi = paymentProcessorDefaultApi;

    public async Task<Result<PaymentProcessorHealthResponse>> PaymentProcessorHealthCheck()
    {
        var response = await _paymentProcessorDefaultApi.PaymentProcessorHealthCheck();
        return !response.IsSuccessful
            ? Result<PaymentProcessorHealthResponse>.Fail(new PaymentProcessorException($"Payment health default error! {response.Error.Message}"))
            : Result<PaymentProcessorHealthResponse>.Ok(response.Content);
    }

    public async Task<Result<PaymentProcessorResponse>> PaymentProcessorAsync(PaymentProcessorContract request)
    {
        var response = await _paymentProcessorDefaultApi.PaymentProcessorAsync(request);
        return !response.IsSuccessful
            ? Result<PaymentProcessorResponse>.Fail(new PaymentProcessorException($"Payment processor default error! {response.Error.Message}"))
            : Result<PaymentProcessorResponse>.Ok(response.Content);
    }
}