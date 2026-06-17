using System.Collections.Concurrent;

namespace Kawai.Api.Robot.Services.Logging;

/// <summary>
/// Singleton service yang menampung log entries di memory (ConcurrentQueue)
/// sebelum di-flush ke database oleh LogFlushBackgroundService.
/// </summary>
public class LogBufferService
{
    private readonly ConcurrentQueue<RequestAMRLogEntry> _requestLogs = new();
    private readonly ConcurrentQueue<ErrorLogEntry> _errorLogs = new();

    /// <summary>
    /// Max body size yang disimpan (dalam characters). Lebih dari ini akan di-truncate.
    /// </summary>
    public const int MaxBodySize = 65536; // 64 KB

    public void EnqueueRequestLog(RequestAMRLogEntry entry)
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

    /// <summary>
    /// Drain semua request logs dari queue. Thread-safe.
    /// </summary>
    public List<RequestAMRLogEntry> DrainRequestLogs(int maxBatchSize = 500)
    {
        var batch = new List<RequestAMRLogEntry>(maxBatchSize);
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

    public int RequestLogCount => _requestLogs.Count;
    public int ErrorLogCount => _errorLogs.Count;
}
