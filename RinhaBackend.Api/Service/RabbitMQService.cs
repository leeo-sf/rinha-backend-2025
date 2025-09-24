using Newtonsoft.Json;
using RabbitMQ.Client;
using RinhaBackend.Api.Contract;
using RinhaBackend.Api.Interface;
using System.Text;

namespace RinhaBackend.Api.Service;

public class RabbitMQService : IRabbitMQService
{
    public async Task PublisherAsync<T>(RabbitMQCredentials credentials, T data, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = credentials.HostName,
            UserName = credentials.UserName,
            Password = credentials.Password,
            VirtualHost = credentials.VirtualHost,
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var messageJson = JsonConvert.SerializeObject(data);
        var body = Encoding.UTF8.GetBytes(messageJson);

        await Task.Run(() => channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body));
    }
}