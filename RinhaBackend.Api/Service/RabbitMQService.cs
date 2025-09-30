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
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var messageJson = JsonConvert.SerializeObject(data);
        var body = Encoding.UTF8.GetBytes(messageJson);

        await Task.Run(() => channel.BasicPublishAsync(string.Empty, queueName, false, new BasicProperties(), body: body));
    }
}