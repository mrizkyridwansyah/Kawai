using System.Collections.Concurrent;

namespace Kawai.Api.Services.Logging;

/// <summary>
/// Singleton service yang menampung log entries di memory (ConcurrentQueue)
/// sebelum di-flush ke database oleh LogFlushBackgroundService.
/// </summary>
public class LogBufferService
{
    private readonly ConcurrentQueue<RequestLogEntry> _requestLogs = new();
    private readonly ConcurrentQueue<ErrorLogEntry> _errorLogs = new();
    private readonly ConcurrentQueue<MQLogEntry> _mqLogs = new();

    /// <summary>
    /// Max body size yang disimpan (dalam characters). Lebih dari ini akan di-truncate.
    /// </summary>
    public const int MaxBodySize = 65536; // 64 KB

    public void EnqueueRequestLog(RequestLogEntry entry)
    {
        // Truncate body jika terlalu besar
        if (entry.RequestBody != null && entry.RequestBody.Length > MaxBodySize)
        {
            entry.RequestBody = entry.RequestBody[..MaxBodySize] + "... [TRUNCATED]";
        }

        _requestLogs.Enqueue(entry);
    }

    public void EnqueueErrorLog(ErrorLogEntry entry)
    {
        _errorLogs.Enqueue(entry);
    }

    public void EnqueueMQLog(MQLogEntry entry)
    {
        _mqLogs.Enqueue(entry);
    }

    /// <summary>
    /// Drain semua request logs dari queue. Thread-safe.
    /// </summary>
    public List<RequestLogEntry> DrainRequestLogs(int maxBatchSize = 500)
    {
        var batch = new List<RequestLogEntry>(maxBatchSize);
        while (batch.Count < maxBatchSize && _requestLogs.TryDequeue(out var entry))
        {
            batch.Add(entry);
        }
        return batch;
    }

    /// <summary>
    /// Drain semua error logs dari queue. Thread-safe.
    /// </summary>
    public List<ErrorLogEntry> DrainErrorLogs(int maxBatchSize = 500)
    {
        var batch = new List<ErrorLogEntry>(maxBatchSize);
        while (batch.Count < maxBatchSize && _errorLogs.TryDequeue(out var entry))
        {
            batch.Add(entry);
        }
        return batch;
    }

    /// <summary>
    /// Drain semua MQ logs dari queue. Thread-safe.
    /// </summary>
    public List<MQLogEntry> DrainMQLogs(int maxBatchSize = 500)
    {
        var batch = new List<MQLogEntry>(maxBatchSize);
        while (batch.Count < maxBatchSize && _mqLogs.TryDequeue(out var entry))
        {
            batch.Add(entry);
        }
        return batch;
    }

    public int RequestLogCount => _requestLogs.Count;
    public int ErrorLogCount => _errorLogs.Count;
    public int MQLogCount => _mqLogs.Count;
}
