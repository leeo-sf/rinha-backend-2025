using MediatR;
using RinhaBackend.Api.Config;
using RinhaBackend.Api.Interface;
using RinhaBackend.Api.MediatR.Request;
using System.Text;
using System.Text.Json;

namespace RinhaBackend.Api.MediatR.Handler;

public class PaymentsHandler
    : IRequestHandler<RequestsPaymentRequest, Result>
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly AppConfiguration _appConfiguration;

    public PaymentsHandler(
        IPaymentsRepository paymentsRepository, 
        IRabbitMQService rabbitMQService,
        AppConfiguration appConfiguration)
    {
        _paymentsRepository = paymentsRepository;
        _rabbitMQService = rabbitMQService;
        _appConfiguration = appConfiguration;
    }

    public async Task<Result> Handle(RequestsPaymentRequest request, CancellationToken cancellationToken)
    {
        await _paymentsRepository.CreatePaymentAsync(new(request.CorrelationId, request.Amount, null, null, false), cancellationToken);
        await _rabbitMQService.PublisherAsync(_appConfiguration.RabbitMQ, request, _appConfiguration.RabbitMQ.Queues.PaymentRequestedQueue);
        return Result.Ok();
    }
}