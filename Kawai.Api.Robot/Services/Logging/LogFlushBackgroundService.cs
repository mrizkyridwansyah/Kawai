using System.Data;
using System.Text;
using Dapper;
using Kawai.Data.SqlConnections;

namespace Kawai.Api.Robot.Services.Logging;

public class LogFlushBackgroundService : BackgroundService
{
    private readonly LogBufferService _buffer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogFlushBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    private readonly TimeSpan _flushInterval;
    private readonly int _maxBatchSize;

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

        var seconds = _configuration.GetValue<int>("LogBuffer:FlushIntervalSeconds");
        _flushInterval = TimeSpan.FromSeconds(seconds == 0 ? 5 : seconds);

        var batchSize = _configuration.GetValue<int>("LogBuffer:MaxBatchSize");
        _maxBatchSize = batchSize == 0 ? 500 : batchSize;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[LogFlush] Background service is starting with interval {Interval}s and batch size {BatchSize}",
            _flushInterval.TotalSeconds, _maxBatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_flushInterval, stoppingToken);
                await FlushAllLogs();
            }
            catch (TaskCanceledException)
            {
                // Normal saat shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LogFlush] Error occurred while flushing logs.");
            }
        }

        // Final flush saat aplikasi dimatikan
        _logger.LogInformation("[LogFlush] Background service is stopping, flushing remaining logs...");
        await FlushAllLogs();
    }

    private async Task FlushAllLogs()
    {
        await FlushRequestLogs();
        await FlushErrorLogs();
    }

    private async Task FlushRequestLogs()
    {
        var batch = _buffer.DrainRequestLogs(_maxBatchSize);
        if (batch.Count == 0) return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var logExecutor = scope.ServiceProvider.GetRequiredService<LogExecutor>();

            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO [dbo].[RequestAMRLogs] ([Method], [Path], [Token], [IP], [UserID], [FullName], [Timestamp], [ElapsedtimeMs], [RequestBody]) VALUES");

            var parameters = new Dapper.DynamicParameters();
            for (int i = 0; i < batch.Count; i++)
            {
                if (i > 0) sql.AppendLine(",");
                sql.Append($"(@Method{i}, @Path{i}, @Token{i}, @IP{i}, @UID{i}, @FN{i}, @TS{i}, @Elapsed{i}, @Body{i})");

                parameters.Add($"Method{i}", batch[i].Method);
                parameters.Add($"Path{i}", batch[i].RequestPath);
                parameters.Add($"Token{i}", batch[i].Token);
                parameters.Add($"IP{i}", batch[i].RemoteAddr);
                parameters.Add($"UID{i}", batch[i].UserID);
                parameters.Add($"FN{i}", batch[i].FullName);
                parameters.Add($"TS{i}", batch[i].Timestamp);
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
}
