using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Data.SqlConnections;

namespace Kawai.Data.Repositories.Mobile;

public class MobileEnableAMRModeRepository: IMobileEnableAMRModeRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileEnableAMRModeRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string itemClass)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", ItemClass = itemClass })).ToList();
    }
    public async Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string lineCode, string itemClass)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_RequestNoDDL";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestNoDto>(sp, new { Keyword = keyword ?? "", LineCode = lineCode, ItemClass = itemClass })).ToList();
    }
    public async Task<TrolleyDto> GetDataTrolley(string trolleyNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_GetDataTrolley";
        return await _dbExecutor.QueryFirstOrDefaultAsync<TrolleyDto>(sp, new { TrolleyNo = trolleyNo });
    }
    public async Task Save(MobileManualTrolleyAssign payload, string userId)
    {
        string sql = "sp_Wms_Mobile_ManualTrolleyAssign_Save";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RequestNo,
            payload.TrolleyNo,
            UserId = userId
        });
    }
    public async Task<Dictionary<string, object>> Capture(string requestNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { RequestNo = requestNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    //public async Task UpdateStatusAMR(string requestNo, string lastStatus)
    //{
    //    string sql = "sp_Wms_Mobile_ManualTrolleyAssign_UpdateStatusAMR";
    //    await _dbExecutor.ExecuteAsync(sql, new
    //    {
    //        RequestNo = requestNo,
    //        LastStatus = lastStatus
    //    });
    //}
    //public async Task<Dictionary<string, object>> CaptureStatusAMR(string requestNo)
    //{
    //    string sp = "sp_Wms_Mobile_ManualTrolleyAssign_CaptureStatusAMR";
    //    var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { RequestNo = requestNo });
    //    if (result == null)
    //        return new Dictionary<string, object>();
    //    return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    //}
}
