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
using System.Diagnostics;

public class TransactionConsumerAsync : BackgroundService
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

    public TransactionConsumerAsync
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
        var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
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

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine("ExecuteAsync Received");
                try
                {
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
                        var stopwatch = Stopwatch.StartNew();

                        message = JsonSerializer.Deserialize<StockTransactionMessage<object>>(json);
                        if (message == null)
                            throw new Exception("Failed to deserialize message.");

                        notification.Title = message.FormatMessage;
                        notification.Description = "Transaction success!";
                        notification.Receiver = message.AuthUserId;
                        notification.Sender = message.AuthUserId;
                        notification.NotifType = "SUCCESS";

                        await ProcessMessageAsync(message);

                        _channel.BasicAck(ea.DeliveryTag, false);

                        stopwatch.Stop();

                        await SaveMQLogs(message, stopwatch.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        notification.NotifType = "ERROR";
                        notification.Description = "Transaction failed: " + ex.Message;
                        await SaveErrorLogs(json, message.LogContext, ex);
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }

                    if (!string.IsNullOrWhiteSpace(notification.Receiver))
                    {
                        await _notificationRepository.SaveNotification(notification);
                        var notifications = new List<Notification> { notification };
                        await _notificationService.BroadCastOnlyTo([notification.Receiver], "NewNotification", new
                        {
                            Count = 1,
                            Notifications = notifications
                        });
                    }
                }
                catch (Exception ex)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
            Console.WriteLine("BasicConsume started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[RABBITMQ] still running ...");
                await Task.Delay(1000, stoppingToken); // sleep to avoid CPU busy loop
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to start TransactionConsumer: {ex.Message}");
        }
    }

    private async Task SaveErrorLogs(string payload, LogContext logContext, Exception ex)
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
            Date = new EpochDateTime(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()).Value,
            ex.Message,
            Method = "BACKGROUND",
            UserAgent = "RabbitMQ/BackgroundWorker",
            logContext.RemoteAddr,
            RequestPath = GetType().Name + ": url(" + logContext.RequestPath + ")",
            RequestBody = payload,
            StackTrace = ex?.InnerException?.StackTrace ?? ex?.StackTrace,
            StatusCode = 500,
            logContext.UserID,
            logContext.FullName
        };

        await _logExecutor.ExecuteAsync(sql, log, commandType: System.Data.CommandType.Text);
    }

    private async Task SaveMQLogs(StockTransactionMessage<object> message, long elapsedTime)
    {
        using var scope = _scopeFactory.CreateScope();
        var _logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

        var sql = @"
                    INSERT INTO [dbo].[MQLogs]
                    ([TimeStamp], [TransactionType], [FormatMessage], [Method], [Path], [UserID], [FullName], [ElapsedtimeMs])
                    VALUES
                    (@TimeStamp, @TransactionType, @FormatMessage, @Method, @RequestPath, @UserID, @FullName, @ElapsedMilliseconds);
                ";

        long timeStamp = EpochDateTime.Now;
        var log = new
        {
            message.TimeStamp,
            message.TransactionType,
            message.FormatMessage,
            Method = "BACKGROUND",
            RequestPath = GetType().Name + ": url(" + message.LogContext.RequestPath + ")",
            message.LogContext.UserID,
            message.LogContext.FullName,
            Date = timeStamp,
            ElapsedMilliseconds = elapsedTime
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
