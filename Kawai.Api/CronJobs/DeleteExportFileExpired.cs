using Hangfire;
using Kawai.Api.Shared;
using Kawai.Data.SqlConnections;
using System.Data;

namespace Kawai.Api.CronJobs;

/// <summary>
/// Cron job ini fungsinya buat hapus hasil file export yg udah lewat dari 1 menit, sumber nya table ExportFile.
/// Biar ga menuhin storage server
/// </summary>
[CronJob("* * * * *")]
[DisableConcurrentExecution(600)]
public class DeleteExportFileExpired: BaseCronJob
{
    private readonly DbExecutor _dbExecutor;
    private readonly IFileStorage _fileStorage;

    public DeleteExportFileExpired(DbExecutor dbExecutor, IFileStorage fileStorage)
    {
        _dbExecutor = dbExecutor;
        _fileStorage = fileStorage;
    }

    public override async Task Run()
    {
        var keys = (await _dbExecutor.QueryListAsync<string>(@"SELECT FileKey FROM ExportFile WHERE DATEDIFF(MINUTE, RegisterDate, GETDATE()) > TTLMinute", commandType: CommandType.Text)).ToList();
        foreach (var key in keys)
        {
            await _dbExecutor.ExecuteAsync(@"DELETE FROM ExportFile WHERE FileKey = @key", new { key }, commandType: CommandType.Text);
            _fileStorage.RemoveFromExports(key);
        }
    }
}
