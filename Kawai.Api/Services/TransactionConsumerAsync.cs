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
    private IConnection? _connection;
    private IModel? _channel;

    private readonly IServiceScopeFactory _scopeFactory;
    private const string ExchangeName = "stock_transaction_exchange";
    private const string QueueName = "stock_transaction_queue";
    private const string RoutingKey = "stock_transaction";
    private const string DlxExchange = "stock_transaction_dlx";
    private const string DlxQueue = "stock_transaction_dead_letter_queue_v1";
    private const string DlxRoutingKey = "dead.stock_transaction";

    private readonly IConfiguration _configuration;

    public TransactionConsumerAsync(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                InitRabbitMq();
                StartConsumer(stoppingToken);

                Console.WriteLine("[RABBITMQ] Consumer started successfully");

                // keep alive
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RABBITMQ] disconnected: {ex.Message}");
                await Task.Delay(5000, stoppingToken); // retry after 5 sec
            }
        }
    }

    private void InitRabbitMq()
    {
        var factory = new RabbitMQ.Client.ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            Port = int.TryParse(_configuration["RabbitMQ:Port"], out var port) ? port : 5672,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.BasicQos(0, 1, false);

        DeclareTopologySafe();
    }

    private void DeclareTopologySafe()
    {
        var dlxQueueArgs = new Dictionary<string, object>
    {
        { "x-message-ttl", 2592000000 }
    };

        var queueArgs = new Dictionary<string, object>
    {
        { "x-dead-letter-exchange", DlxExchange },
        { "x-dead-letter-routing-key", DlxRoutingKey }
    };

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);
        _channel.ExchangeDeclare(DlxExchange, ExchangeType.Direct, durable: true);

        _channel.QueueDeclare(
            QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs
        );

        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        _channel.QueueDeclare(
            DlxQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: dlxQueueArgs
        );

        _channel.QueueBind(DlxQueue, DlxExchange, DlxRoutingKey);
    }

    private void StartConsumer(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            await HandleMessage(ea);
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    private async Task HandleMessage(BasicDeliverEventArgs ea)
    {
        using var scope = _scopeFactory.CreateScope();

        var notificationRepository =
            scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var notificationService =
            scope.ServiceProvider.GetRequiredService<NotificationService<NotifApprovalHub>>();

        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        StockTransactionMessage<object>? message = null;

        var notification = new Notification
        {
            Priority = "TOP",
            UrlRedirect = ""
        };

        try
        {
            var stopwatch = Stopwatch.StartNew();

            message = JsonSerializer.Deserialize<StockTransactionMessage<object>>(json);

            if (message == null)
                throw new Exception("Invalid message payload");

            notification.Title = message.FormatMessage;
            notification.Receiver = message.AuthUserId;

            Console.WriteLine($"[RABBITMQ] HandleMessage: {message.FormatMessage}");

            await ProcessMessageAsync(message);

            _channel!.BasicAck(ea.DeliveryTag, false);

            stopwatch.Stop();

            _ = Task.Run(() => SaveMQLogs(message, stopwatch.ElapsedMilliseconds));

            notification.Description = "Transaction success!";
            notification.NotifType = "SUCCESS";
        }
        catch (Exception ex)
        {
            _channel!.BasicNack(ea.DeliveryTag, false, false);

            notification.NotifType = "ERROR";
            notification.Description = "Transaction failed: " + ex.Message;

            _ = Task.Run(() => SaveErrorLogs(json, message?.LogContext, ex));
        }

        if (!string.IsNullOrWhiteSpace(notification.Receiver))
        {
            await notificationRepository.SaveNotification(notification);
            var notifications = new List<Notification> { notification };

            if (message.BroadcastBaseOn == "TOKEN" && !String.IsNullOrEmpty(message.Token))
            {
                await notificationService.BroadCastOnlyTo([message.Token], "NewNotification",
                    new
                    {
                        Count = 1,
                        Notifications = notifications
                    }
                );
            }
            else
            {
                await notificationService.BroadCastOnlyTo([notification.Receiver], "NewNotification",
                    new
                    {
                        Count = 1,
                        Notifications = notifications
                    }
                );
            }
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
        var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<ITransactionHandler>>().ToDictionary(h => h.TransactionType.ToUpper(), h => h);

        if (handlers.TryGetValue(message.TransactionType.ToUpper(), out var handler))
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
