using Kawai.Domain.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace Kawai.Api.Services;

public interface ITransactionProducer
{
    void Publish<T>(StockTransactionMessage<T> message);
}

public class TransactionProducer : ITransactionProducer, IDisposable
{
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;
    private readonly string _dlxExchange;
    private readonly string _dlxRoutingKey;

    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new();
    private readonly ConnectionFactory _factory;

    public TransactionProducer(IConfiguration configuration)
    {
        var prefix = configuration["RabbitMQ:QueuePrefix"] ?? "dev";

        _exchangeName = $"stock_{prefix}_transaction_exchange";
        _queueName = $"stock_{prefix}_transaction_queue";
        _routingKey = $"stock_{prefix}_transaction";
        _dlxExchange = $"stock_{prefix}_transaction_dlx";
        _dlxRoutingKey = $"dead.stock_{prefix}_transaction";

        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            Port = int.TryParse(configuration["RabbitMQ:Port"], out var port) ? port : 5672,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
    }

    private void EnsureConnection()
    {
        if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            return;

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ConfirmSelect();

        DeclareTopology();
    }

    private void DeclareTopology()
    {
        var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", _dlxExchange },
            { "x-dead-letter-routing-key", _dlxRoutingKey }
        };

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(
            _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs
        );
        _channel.QueueBind(_queueName, _exchangeName, _routingKey);
    }

    public void Publish<T>(StockTransactionMessage<T> message)
    {
        EnsureConnection();

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        lock (_lock) // IMPORTANT: channel is not thread-safe
        {
            if (!_connection.IsOpen || !_channel.IsOpen)
                throw new Exception("RabbitMQ connection/channel is not open");

            int retry = 0;

            while (true)
            {
                try
                {
                    _channel.BasicPublish(_exchangeName, _routingKey, props, body);

                    if (!_channel.WaitForConfirms(TimeSpan.FromSeconds(5)))
                        throw new Exception("RabbitMQ publish not confirmed");

                    return; // success
                }
                catch
                {
                    retry++;

                    if (retry >= 3)
                        throw;

                    Thread.Sleep(100); // small backoff
                }
            }
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch
        {
            // ignore dispose errors
        }
    }
}
