using Kawai.Domain.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;
using Kawai.Api.Services;
using Kawai.Api.Hub;
using Kawai.Api.Shared.Handlers;
using Kawai.Domain;

public class TransactionConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    private readonly IServiceScopeFactory _scopeFactory;
    private Dictionary<string, ITransactionHandler> _handlers;

    private const string ExchangeName = "stock_transaction_exchange";
    private const string QueueName = "stock_transaction_queue";
    private const string RoutingKey = "stock_transaction";
    private const string DlxExchange = "stock_transaction_dlx";
    private const string DlxQueue = "stock_transaction_dead_letter_queue";
    private const string DlxRoutingKey = "dead.stock_transaction";

    public TransactionConsumer
    (
        IServiceScopeFactory scopeFactory
    )
    {
        _scopeFactory = scopeFactory;
        InitRabbitMq();
    }

    private void InitRabbitMq()
    {
        Console.WriteLine("InitRabbitMq");
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
                    SaveErrorLogs(json, ex).GetAwaiter().GetResult();
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

    private async Task SaveErrorLogs(string payload, Exception ex)
    {
        using var scope = _scopeFactory.CreateScope();
        var _logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

        var sql = @"
                    INSERT INTO ErrorLogs
                    (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode)
                    VALUES
                    (@Date, @Message, @Method, @UserAgent, @RemoteAddr, @RequestPath, @RequestBody, @StackTrace, @UserId, @FullName, @StatusCode);
                ";

        var log = new
        {
            Date = new EpochDateTime(DateTime.UtcNow.ToUnixTimeMilliseconds()).Value,
            ex.Message,
            Method = "BACKGROUND",
            UserAgent = "RabbitMQ/BackgroundWorker",
            RemoteAddr = "-",
            RequestPath = GetType().Name,
            RequestBody = payload,
            StackTrace = ex?.InnerException?.StackTrace ?? ex?.StackTrace,
            StatusCode = 500,
            UserID = "",
            FullName = ""
        };

        await _logExecutor.ExecuteAsync(sql, log, commandType: System.Data.CommandType.Text);
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
