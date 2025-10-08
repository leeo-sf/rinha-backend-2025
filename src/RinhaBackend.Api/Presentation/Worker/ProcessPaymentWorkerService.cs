using Microsoft.Extensions.Caching.Memory;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RinhaBackend.Api.Application;
using RinhaBackend.Api.Application.Config;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Request;
using RinhaBackend.Api.Application.Response;
using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Enum;
using RinhaBackend.Api.Domain.Interface;
using System.Text;
using System.Text.Json;

namespace RinhaBackend.Api.Presentation.Worker;

public class ProcessPaymentWorkerService : BackgroundService
{
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private const string CACHE_KEY_DEFAULT = "PaymentProcessorDefaultIsFailing";
    private const string CACHE_KEY_FALLBACK = "PaymentProcessorFallbackIsFailing";
    private readonly AppConfiguration _appConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly PaymentProcessorFactory _paymentProcessorFactory;
    private readonly IRabbitMQConnection _rabbitMqConnection;
    private IChannel? _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcessPaymentWorkerService(
        ILogger<ProcessPaymentWorkerService> logger,
        AppConfiguration appConfiguration,
        IMemoryCache memoryCache,
        PaymentProcessorFactory paymentProcessorFactory,
        IRabbitMQConnection rabbitMqConnection,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _appConfiguration = appConfiguration;
        _memoryCache = memoryCache;
        _paymentProcessorFactory = paymentProcessorFactory;
        _rabbitMqConnection = rabbitMqConnection;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbitMqConnection.CreateChannelAsync(stoppingToken);
        await _channel.QueueDeclareAsync(queue: _appConfiguration.RabbitMQ.Queues.PaymentRequestedQueue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var message = JsonSerializer.Deserialize<PaymentContract>(content)
                ?? throw new ApplicationException("Error deserialize message to process payment.");

            try
            {
                _logger.LogInformation($"Starting payment processing {message.CorrelationId}");
                await ProcessPaymentAsync(message);
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment {message.CorrelationId}");
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };
        await _channel.BasicConsumeAsync(queue: _appConfiguration.RabbitMQ.Queues.PaymentRequestedQueue, autoAck: false, consumer: consumer);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessPaymentAsync(PaymentContract paymentContract)
    {
        var requestedAt = DateTime.UtcNow;
        var payment = new Payment(paymentContract.CorrelationId, paymentContract.Amount, requestedAt, PaymentProcessorEnum.NONE);

        if (_memoryCache.Get<bool>(CACHE_KEY_DEFAULT))
        {
            var responseFallback = await SendPaymentProcessing(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!), PaymentProcessorEnum.FALLBACK);
            if (responseFallback.IsSuccess)
            {
                await InsertProcessedPayment(payment with { ProcessedBy = PaymentProcessorEnum.FALLBACK });
                return;
            }
        }

        var responseDefault = await SendPaymentProcessing(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!), PaymentProcessorEnum.DEFAULT);
        if (!responseDefault.IsSuccess)
        {
            if (!_memoryCache.Get<bool>(CACHE_KEY_FALLBACK))
            {
                var responseFallback = await SendPaymentProcessing(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!), PaymentProcessorEnum.FALLBACK);
                if (responseFallback.IsSuccess)
                {
                    await InsertProcessedPayment(payment with { ProcessedBy = PaymentProcessorEnum.FALLBACK });
                    return;
                }
            }
        }
        else
        {
            await InsertProcessedPayment(payment with { ProcessedBy = PaymentProcessorEnum.DEFAULT });
            return;
        }

        throw new ApplicationException("Unable to process payment on any service.");
    }

    private async Task<Result<PaymentProcessorResponse>> SendPaymentProcessing(
        PaymentProcessorRequest request,
        PaymentProcessorEnum paymentProcessorType)
    {
        var processor = _paymentProcessorFactory.CreateFactory(paymentProcessorType);
        return await processor.PaymentProcessorAsync(request);
    }

    private async Task InsertProcessedPayment(Payment payment)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentsRepository>();
            await paymentRepository.CreatePaymentAsync(payment, CancellationToken.None);
        }
    }
}