using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RinhaBackend.Api.Config;
using RinhaBackend.Api.Contract;
using RinhaBackend.Api.Data.Entity;
using RinhaBackend.Api.Interface;
using System.Text;
using System.Text.Json;

namespace RinhaBackend.Api.Worker;

public class PaymentProcessedWorkerService : BackgroundService
{
    private readonly ILogger<ProcessPaymentWorkerService> _logger;
    private readonly AppConfiguration _appConfiguration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMQConnection _rabbitMqConnection;
    private IChannel? _channel;

    public PaymentProcessedWorkerService(
        ILogger<ProcessPaymentWorkerService> logger,
        AppConfiguration appConfiguration,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMQConnection rabbitMqConnection)
    {
        _logger = logger;
        _appConfiguration = appConfiguration;
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbitMqConnection.CreateChannelAsync(stoppingToken);
        await _channel.QueueDeclareAsync(queue: _appConfiguration.RabbitMQ.Queues.PaymentProcessedQueue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var payment = JsonSerializer.Deserialize<PaymentProcessedContract>(content)
                ?? throw new ApplicationException("Error deserialize message to set payment as paid");

            try
            {
                _logger.LogInformation($"Starting process to download payment {payment.CorrelationId}");
                await UpdateProcessedPayment(new(payment.CorrelationId, payment.Amount, payment.RequestedAt, payment.ProcessedBy, payment.IsProcessed));
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when downloading payment {payment.CorrelationId}");
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };
        await _channel.BasicConsumeAsync(queue: _appConfiguration.RabbitMQ.Queues.PaymentProcessedQueue, autoAck: false, consumer: consumer);
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

    private async Task UpdateProcessedPayment(Payment payment)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentsRepository>();
            await paymentRepository.UpdatePaymentToProcessed(payment, CancellationToken.None);
        }
    }
}