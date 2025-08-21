using Kawai.Domain.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;
using Kawai.Api.Services;
using Kawai.Api.Hub;
using System.Diagnostics;
using Kawai.Data.Repositories;
using DocumentFormat.OpenXml.EMMA;
using Kawai.Domain.DTOs.Log;
using Kawai.Api;
using Kawai.Api.Shared.Handlers;

public class TransactionConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    private readonly LogExecutor _logExecutor;
    private readonly NotificationService<NotifApprovalHub> _notificationService;
    private readonly Dictionary<string, ITransactionHandler> _handlers;
    private readonly INotificationRepository _notificationRepository;

    private const string ExchangeName = "stock_transaction_exchange";
    private const string QueueName = "stock_transaction_queue";
    private const string RoutingKey = "stock_transaction";

    public TransactionConsumer
    (
        LogExecutor logExecutor,
        NotificationService<NotifApprovalHub> notificationService,
        INotificationRepository notificationRepository,
        IEnumerable<ITransactionHandler> handlers
    )
    {
        _logExecutor = logExecutor;
        _notificationService = notificationService;
        _notificationRepository = notificationRepository;
        _handlers = handlers.ToDictionary(h => h.TransactionType.ToUpper(), h => h);
        InitRabbitMq();
    }

    private void InitRabbitMq()
    {
        var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        _channel.BasicQos(0, 1, false); // process 1 message at a time
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
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

                await ProcessMessageAsync(message);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                notification.NotifType = "ERROR";
                await SaveErrorLogs(json, ex);
                await _notificationRepository.SaveNotification(notification);
                //_channel.BasicNack(ea.DeliveryTag, false, true);
            }

            List<Notification> notifications = [notification];
            if (!string.IsNullOrWhiteSpace(notification.Receiver))
            {
                await _notificationService.BroadCastOnlyTo([notification.Receiver], "NewNotification", new
                {
                    Count = 1,
                    Notifications = notifications
                });
            }
        };

        _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task SaveErrorLogs(string payload, Exception ex)
    {
        var sql = @"
                    INSERT INTO ErrorLogs
                    (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode)
                    VALUES
                    (@Date, @Message, @Method, @UserAgent, @RemoteAddr, @RequestPath, @RequestBody, @StackTrace, @UserId, @FullName, @StatusCode);
                ";

        var log = new
        {
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
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
        if (_handlers.TryGetValue(message.TransactionType.ToUpper(), out var handler))
        {
            await handler.HandleAsync(message.Payload, message.AuthUserId);
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
