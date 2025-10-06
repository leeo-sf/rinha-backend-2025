using MediatR;
using RinhaBackend.Api.Application;
using RinhaBackend.Api.Application.Config;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Extensions;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Domain.Interface;

namespace RinhaBackend.Api.Domain.Handler;

public class PaymentsHandler
    : IRequestHandler<RequestsPaymentRequest, Result>,
        IRequestHandler<SummaryOfProcessedPaymentsRequest, Result<PaymentsProcessedAtIntervalsContract>>
{
    private readonly ILogger<PaymentsHandler> _logger;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly AppConfiguration _appConfiguration;

    public PaymentsHandler(
        ILogger<PaymentsHandler> logger,
        IPaymentsRepository paymentsRepository, 
        IRabbitMQService rabbitMQService,
        AppConfiguration appConfiguration)
    {
        _logger = logger;
        _paymentsRepository = paymentsRepository;
        _rabbitMQService = rabbitMQService;
        _appConfiguration = appConfiguration;
    }

    public async Task<Result> Handle(RequestsPaymentRequest request, CancellationToken cancellationToken)
    {
        await _paymentsRepository.CreatePaymentAsync(new(request.CorrelationId, request.Amount, null, null, false), cancellationToken);
        await _rabbitMQService.PublisherAsync(_appConfiguration.RabbitMQ, request, _appConfiguration.RabbitMQ.Queues.PaymentRequestedQueue);
        _logger.LogInformation($"Requested payment '{request.CorrelationId}' in the amount '${request.Amount}'");
        return Result.Ok();
    }

    public async Task<Result<PaymentsProcessedAtIntervalsContract>> Handle(SummaryOfProcessedPaymentsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for processed payments");
        var payments = await _paymentsRepository.PaymentsProcessedAsync(request.From, request.To, cancellationToken);
        return Result<PaymentsProcessedAtIntervalsContract>.Ok(payments.BuildSummary());
    }
}