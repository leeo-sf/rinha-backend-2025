using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Service;
using RinhaBackend.Api.Domain.Enum;

namespace RinhaBackend.Api.Application.Factory;

public class PaymentProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessorFactory(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public IPaymentProcessor CreateFactory(PaymentProcessorEnum paymentProcessorType)
        => paymentProcessorType switch
        {
            PaymentProcessorEnum.DEFAULT => ActivatorUtilities.CreateInstance<DefaultPaymentProcessor>(_serviceProvider),
            PaymentProcessorEnum.FALLBACK => ActivatorUtilities.CreateInstance<FallbackPaymentProcessor>(_serviceProvider),
            _ => throw new ArgumentOutOfRangeException(nameof(paymentProcessorType), paymentProcessorType, null)
        };
}