using Microsoft.Extensions.Caching.Memory;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RinhaBackend.Api.Config;
using RinhaBackend.Api.Contract;
using RinhaBackend.Api.Contract.Enum;
using RinhaBackend.Api.Interface;
using System.Text;
using System.Text.Json;

namespace RinhaBackend.Api.Worker;

public class ProcessPaymentWorkerService : BackgroundService
{
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private readonly IRabbitMQService _rabbitMQService;
    private const string CACHE_KEY_DEFAULT = "PaymentProcessorDefaultIsFailing";
    private const string CACHE_KEY_FALLBACK = "PaymentProcessorFallbackIsFailing";
    private readonly AppConfiguration _appConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly IPaymentProcessorDefaultApiService _paymentProcessorDefaultApiService;
    private readonly IPaymentProcessorFallbackApiService _paymentProcessorFallbackApiService;
    private readonly IRabbitMQConnection _rabbitMqConnection;
    private IChannel? _channel;

    public ProcessPaymentWorkerService(
        ILogger<ProcessPaymentWorkerService> logger,
        IRabbitMQService rabbitMQService,
        AppConfiguration appConfiguration,
        IMemoryCache memoryCache,
        IPaymentProcessorDefaultApiService paymentProcessorDefaultApiService,
        IPaymentProcessorFallbackApiService paymentProcessorFallbackApiService,
        IRabbitMQConnection rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMQService = rabbitMQService;
        _appConfiguration = appConfiguration;
        _memoryCache = memoryCache;
        _paymentProcessorDefaultApiService = paymentProcessorDefaultApiService;
        _paymentProcessorFallbackApiService = paymentProcessorFallbackApiService;
        _rabbitMqConnection = rabbitMqConnection;
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
        var payment = new PaymentProcessedContract(paymentContract.CorrelationId, paymentContract.Amount, requestedAt, false, ProcessedByEnum.NONE);

        if (_memoryCache.Get<bool>(CACHE_KEY_DEFAULT))
        {
            var responseFallback = await _paymentProcessorFallbackApiService.PaymentProcessorAsync(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!));
            if (responseFallback.IsSuccess)
            {
                await PubPaymentToBeDischarged(payment with { ProcessedBy = ProcessedByEnum.FALLBACK, IsProcessed = true });
                return;
            }
        }

        var responseDefault = await _paymentProcessorDefaultApiService.PaymentProcessorAsync(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!));
        if (!responseDefault.IsSuccess)
        {
            if (!_memoryCache.Get<bool>(CACHE_KEY_FALLBACK))
            {
                var responseFallback = await _paymentProcessorFallbackApiService.PaymentProcessorAsync(new(payment.CorrelationId, payment.Amount, (DateTime)payment.RequestedAt!));
                if (responseFallback.IsSuccess)
                {
                    await PubPaymentToBeDischarged(payment with { ProcessedBy = ProcessedByEnum.FALLBACK, IsProcessed = true });
                    return;
                }
            }
        }
        else
        {
            await PubPaymentToBeDischarged(payment with { ProcessedBy = ProcessedByEnum.DEFAULT, IsProcessed = true });
            return;
        }

        throw new ApplicationException("Unable to process payment on any service.");
    }

    private async Task PubPaymentToBeDischarged(PaymentProcessedContract payment)
        => await _rabbitMQService.PublisherAsync(_appConfiguration.RabbitMQ, payment, _appConfiguration.RabbitMQ.Queues.PaymentProcessedQueue);
}