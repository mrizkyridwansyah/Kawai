using Hangfire;
using Kawai.Api.Shared;
using Kawai.Data.SqlConnections;
using System.Data;

namespace Kawai.Api.CronJobs;

/// <summary>
/// Cron job untuk menghapus log lama di database secara otomatis
/// agar storage server tidak penuh.
/// Menghapus RequestLogs (> 30 hari), MQLogs (> 90 hari), ErrorLogs (> 180 hari)
/// </summary>
[CronJob("0 2 * * *")] // Jalan setiap jam 2 pagi
[DisableConcurrentExecution(3600)]
public class DeleteExpiredLogsCronJob : BaseCronJob
{
    private readonly LogExecutor _logExecutor;

    public DeleteExpiredLogsCronJob(LogExecutor logExecutor)
    {
        _logExecutor = logExecutor;
    }

    public override async Task Run()
    {
        var now = DateTimeOffset.UtcNow;
        int limit = -180;
        var dateLimit = now.AddDays(limit).ToUnixTimeMilliseconds();

        // 1. Bersihkan RequestLogs (> 180 hari)
        await _logExecutor.ExecuteAsync(@"
            DELETE FROM RequestLogs 
            WHERE Timestamp < @Limit",
            new { Limit = dateLimit },
            commandType: CommandType.Text);

        // 2. Bersihkan MQLogs (> 180 hari)
        await _logExecutor.ExecuteAsync(@"
            DELETE FROM MQLogs 
            WHERE TimeStamp < @Limit",
            new { Limit = dateLimit },
            commandType: CommandType.Text);

        // 3. Bersihkan ErrorLogs (> 180 hari)
        await _logExecutor.ExecuteAsync(@"
            DELETE FROM ErrorLogs 
            WHERE Date < @Limit",
            new { Limit = dateLimit },
            commandType: CommandType.Text);

        // 4. Bersihkan RequestAMRLogs (> 180 hari)
        await _logExecutor.ExecuteAsync(@"
            DELETE FROM RequestAMRLogs 
            WHERE Timestamp < @Limit",
            new { Limit = dateLimit },
            commandType: CommandType.Text);
    }
}
