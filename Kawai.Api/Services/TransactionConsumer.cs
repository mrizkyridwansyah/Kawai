using Kawai.Domain.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;
using Kawai.Api.Services;
using Kawai.Api.Services.Logging;
using Kawai.Api.Hub;
using Kawai.Api.Shared.Handlers;
using Kawai.Domain;

public class TransactionConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    private readonly IServiceScopeFactory _scopeFactory;
    private Dictionary<string, ITransactionHandler> _handlers;
    private readonly LogBufferService _logBuffer;

    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;
    private readonly string _dlxExchange;
    private readonly string _dlxRoutingKey;

    private readonly IConfiguration _configuration;

    public TransactionConsumer
    (
        IServiceScopeFactory scopeFactory,
        LogBufferService logBuffer,
        IConfiguration configuration
    )
    {
        _scopeFactory = scopeFactory;
        _logBuffer = logBuffer;
        _configuration = configuration;
        InitRabbitMq();
    }

    private void InitRabbitMq()
    {
        Console.WriteLine("InitRabbitMq");

        var prefix = _configuration["RabbitMQ:QueuePrefix"] ?? "dev";

        _exchangeName = $"stock_{prefix}_transaction_exchange";
        _queueName = $"stock_{prefix}_transaction_queue";
        _routingKey = $"stock_{prefix}_transaction";
        _dlxExchange = $"stock_{prefix}_transaction_dlx";
        _dlxRoutingKey = $"dead.stock_{prefix}_transaction";

        var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // dlx ini untuk simpan message yg error / NACK tapi ga di requeue. jadi masih bisa diliat, tapi gw kasih batasan 1 bulan.
        var dlxQueueArgs = new Dictionary<string, object>
        {
            { "x-message-ttl", 2592000000 }, // 1 bulan dalam ms
        };
        _channel.ExchangeDeclare(DlxExchange, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(DlxQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(DlxQueue, DlxExchange, DlxRoutingKey);

        // argumen dlx ini harus disertain ke queue utama biar kalo nack bakal dikirim kesitu.
        var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DlxExchange },
            { "x-dead-letter-routing-key", DlxRoutingKey }
        };

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        _channel.BasicQos(0, 1, false); // process 1 message at a time
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine("ExecuteAsync");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("ExecuteAsync Received");

                using var scope = _scopeFactory.CreateScope();
                var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var _notificationService = scope.ServiceProvider.GetRequiredService<NotificationService<NotifApprovalHub>>();

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                StockTransactionMessage<object> message = null;

                Notification notification = new Notification
                {
                    Priority = "TOP",
                    UrlRedirect = ""
                };

                try
                {
                    message = JsonSerializer.Deserialize<StockTransactionMessage<object>>(json);

                    notification.Title = message.FormatMessage;
                    notification.Receiver = message.AuthUserId;
                    notification.Sender = message.AuthUserId;
                    notification.NotifType = "INFO";

                    ProcessMessageAsync(message).GetAwaiter().GetResult();

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    notification.NotifType = "ERROR";
                    _logBuffer.EnqueueErrorLog(new ErrorLogEntry
                    {
                        Date = new EpochDateTime(DateTime.UtcNow.ToUnixTimeMilliseconds()).Value,
                        Message = ex.Message,
                        Method = "BACKGROUND",
                        UserAgent = "RabbitMQ/BackgroundWorker",
                        RemoteAddr = "-",
                        RequestPath = GetType().Name,
                        RequestBody = json,
                        StackTrace = ex?.InnerException?.StackTrace ?? ex?.StackTrace,
                        StatusCode = 500,
                        UserId = "",
                        FullName = ""
                    });
                    _notificationRepository.SaveNotification(notification).GetAwaiter().GetResult();
                    //_channel.BasicNack(ea.DeliveryTag, false, true);
                }

                if (!string.IsNullOrWhiteSpace(notification.Receiver))
                {
                    var notifications = new List<Notification> { notification };
                    _notificationService.BroadCastOnlyTo([notification.Receiver], "NewNotification", new
                    {
                        Count = 1,
                        Notifications = notifications
                    }).GetAwaiter().GetResult();
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
            Console.WriteLine("BasicConsume started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                //Console.WriteLine($"[RABBITMQ] still running ...");
                await Task.Delay(1000, stoppingToken); // sleep to avoid CPU busy loop
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to start TransactionConsumer: {ex.Message}");
            //throw;
        }


        //return Task.CompletedTask;
    }

    private async Task ProcessMessageAsync(StockTransactionMessage<object> message)
    {
        using var scope = _scopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<ITransactionHandler>>();

        _handlers = handlers.ToDictionary(h => h.TransactionType.ToUpper(), h => h);

        if (_handlers.TryGetValue(message.TransactionType.ToUpper(), out var handler))
        {
            await handler.HandleAsync(message.Payload, message.LogContext, message.AuthUserId);
        }
        else
        {
            throw new InvalidOperationException($"No handler found for transaction type: {message.TransactionType}");
        }

        await Task.Delay(500);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
