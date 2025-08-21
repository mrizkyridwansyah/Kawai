using Kawai.Domain.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Kawai.Api.Services;

public interface ITransactionProducer
{
    void Publish<T>(StockTransactionMessage<T> message);
}

public class TransactionProducer : ITransactionProducer, IDisposable
{
    private readonly string _exchangeName = "stock_transaction_exchange";
    private readonly string _queueName = "stock_transaction_queue";
    private readonly string _routingKey = "stock_transaction";
    private readonly IConnection _connection;
    private readonly IModel _channel;
    public TransactionProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchange, queue and bind once during construction
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(_queueName, _exchangeName, _routingKey);
    }

    public void Publish<T>(StockTransactionMessage<T> message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(_exchangeName, _routingKey, props, body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
