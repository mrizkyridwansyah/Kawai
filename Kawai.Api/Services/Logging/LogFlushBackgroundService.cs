using Kawai.Data.SqlConnections;
using System.Data;
using System.Text;

namespace Kawai.Api.Services.Logging;

/// <summary>
/// BackgroundService yang secara periodik flush log buffer ke database.
/// Pada StopAsync (graceful shutdown), sisa buffer akan di-flush sebelum app mati.
/// </summary>
public class LogFlushBackgroundService : BackgroundService
{
    private readonly LogBufferService _buffer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogFlushBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    private int _flushIntervalSeconds;
    private int _maxBatchSize;

    public LogFlushBackgroundService(
        LogBufferService buffer,
        IServiceScopeFactory scopeFactory,
        ILogger<LogFlushBackgroundService> logger,
        IConfiguration configuration)
    {
        _buffer = buffer;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;

        _flushIntervalSeconds = int.TryParse(_configuration["LogBuffer:FlushIntervalSeconds"], out var interval) ? interval : 5;
        _maxBatchSize = int.TryParse(_configuration["LogBuffer:MaxBatchSize"], out var batch) ? batch : 500;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[LogFlush] Background service started. Flush interval: {Interval}s, Max batch: {Batch}",
            _flushIntervalSeconds, _maxBatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_flushIntervalSeconds), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // App shutting down — break dan flush sisa di StopAsync
                break;
            }

            await FlushAll();
        }
    }

    /// <summary>
    /// Graceful shutdown: flush sisa buffer ke database sebelum app mati.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[LogFlush] Shutting down — flushing remaining buffer (Request: {R}, Error: {E}, MQ: {M})",
            _buffer.RequestLogCount, _buffer.ErrorLogCount, _buffer.MQLogCount);

        await FlushAll();

        _logger.LogInformation("[LogFlush] Shutdown flush complete.");

        await base.StopAsync(cancellationToken);
    }

    private async Task FlushAll()
    {
        await FlushRequestLogs();
        await FlushErrorLogs();
        await FlushMQLogs();
    }

    private async Task FlushRequestLogs()
    {
        var batch = _buffer.DrainRequestLogs(_maxBatchSize);
        if (batch.Count == 0) return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

            // Batch insert menggunakan parameterized VALUES
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO [dbo].[RequestLogs] ([Method], [Path], [Token], [IP], [UserID], [FullName], [Timestamp], [ElapsedtimeMs], [RequestBody]) VALUES");

            var parameters = new Dapper.DynamicParameters();
            for (int i = 0; i < batch.Count; i++)
            {
                if (i > 0) sql.AppendLine(",");
                sql.Append($"(@Method{i}, @Path{i}, @Token{i}, @IP{i}, @UserID{i}, @FullName{i}, @Timestamp{i}, @Elapsed{i}, @Body{i})");

                parameters.Add($"Method{i}", batch[i].Method);
                parameters.Add($"Path{i}", batch[i].RequestPath);
                parameters.Add($"Token{i}", batch[i].Token);
                parameters.Add($"IP{i}", batch[i].RemoteAddr);
                parameters.Add($"UserID{i}", batch[i].UserID);
                parameters.Add($"FullName{i}", batch[i].FullName);
                parameters.Add($"Timestamp{i}", batch[i].Timestamp);
                parameters.Add($"Elapsed{i}", batch[i].ElapsedMilliseconds);
                parameters.Add($"Body{i}", batch[i].RequestBody);
            }

            await logExecutor.ExecuteAsync(sql.ToString(), parameters, commandType: CommandType.Text);

            _logger.LogDebug("[LogFlush] Flushed {Count} request logs", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LogFlush] Failed to flush {Count} request logs", batch.Count);
        }
    }

    private async Task FlushErrorLogs()
    {
        var batch = _buffer.DrainErrorLogs(_maxBatchSize);
        if (batch.Count == 0) return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO ErrorLogs (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode) VALUES");

            var parameters = new Dapper.DynamicParameters();
            for (int i = 0; i < batch.Count; i++)
            {
                if (i > 0) sql.AppendLine(",");
                sql.Append($"(@Date{i}, @Msg{i}, @Method{i}, @UA{i}, @IP{i}, @Path{i}, @Body{i}, @Stack{i}, @UID{i}, @FN{i}, @SC{i})");

                parameters.Add($"Date{i}", batch[i].Date);
                parameters.Add($"Msg{i}", batch[i].Message);
                parameters.Add($"Method{i}", batch[i].Method);
                parameters.Add($"UA{i}", batch[i].UserAgent);
                parameters.Add($"IP{i}", batch[i].RemoteAddr);
                parameters.Add($"Path{i}", batch[i].RequestPath);
                parameters.Add($"Body{i}", batch[i].RequestBody);
                parameters.Add($"Stack{i}", batch[i].StackTrace);
                parameters.Add($"UID{i}", batch[i].UserId);
                parameters.Add($"FN{i}", batch[i].FullName);
                parameters.Add($"SC{i}", batch[i].StatusCode);
            }

            await logExecutor.ExecuteAsync(sql.ToString(), parameters, commandType: CommandType.Text);

            _logger.LogDebug("[LogFlush] Flushed {Count} error logs", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LogFlush] Failed to flush {Count} error logs", batch.Count);
        }
    }

    private async Task FlushMQLogs()
    {
        var batch = _buffer.DrainMQLogs(_maxBatchSize);
        if (batch.Count == 0) return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO [dbo].[MQLogs] ([TimeStamp], [TransactionType], [FormatMessage], [Method], [Path], [UserID], [FullName], [ElapsedtimeMs]) VALUES");

            var parameters = new Dapper.DynamicParameters();
            for (int i = 0; i < batch.Count; i++)
            {
                if (i > 0) sql.AppendLine(",");
                sql.Append($"(@TS{i}, @TT{i}, @FM{i}, @Method{i}, @Path{i}, @UID{i}, @FN{i}, @Elapsed{i})");

                parameters.Add($"TS{i}", batch[i].TimeStamp);
                parameters.Add($"TT{i}", batch[i].TransactionType);
                parameters.Add($"FM{i}", batch[i].FormatMessage);
                parameters.Add($"Method{i}", batch[i].Method);
                parameters.Add($"Path{i}", batch[i].RequestPath);
                parameters.Add($"UID{i}", batch[i].UserID);
                parameters.Add($"FN{i}", batch[i].FullName);
                parameters.Add($"Elapsed{i}", batch[i].ElapsedMilliseconds);
            }

            await logExecutor.ExecuteAsync(sql.ToString(), parameters, commandType: CommandType.Text);

            _logger.LogDebug("[LogFlush] Flushed {Count} MQ logs", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LogFlush] Failed to flush {Count} MQ logs", batch.Count);
        }
    }
}
