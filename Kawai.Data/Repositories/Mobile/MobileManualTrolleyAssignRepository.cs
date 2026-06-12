using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Data.SqlConnections;

namespace Kawai.Data.Repositories.Mobile;

public class MobileManualTrolleyAssignRepository: IMobileManualTrolleyAssignRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileManualTrolleyAssignRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string itemClass)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", ItemClass = itemClass })).ToList();
    }

    public async Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string itemClass, string lineCode)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_RequestNoDDL";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestNoDto>(sp, new { Keyword = keyword ?? "", LineCode = lineCode, ItemClass = itemClass })).ToList();
    }

    public async Task<ManualTrolleyAssignDto> GetDataRequest(string requestNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_GetDataRequest";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ManualTrolleyAssignDto>(sp, new { RequestNo = requestNo });
    }

    public async Task<TrolleyDto> GetDataTrolley(string requestNo, string trolleyNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_GetDataTrolley";
        return await _dbExecutor.QueryFirstOrDefaultAsync<TrolleyDto>(sp, new { RequestNo = requestNo, TrolleyNo = trolleyNo });
    }

    public async Task<ManualTrolleyAssignValidationDto> CheckValidation(MobileManualTrolleyAssign payload)
    {
        string sql = "sp_Wms_Mobile_ManualTrolleyAssign_CheckValidation";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ManualTrolleyAssignValidationDto>(sql, new
        {
            payload.RequestNo,
            payload.TrolleyNo
        });
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

    public async Task SendRequestCancelAMR(string requestNo, string trolleyNo, string userId)
    {
        string sql = "sp_Wms_Mobile_ManualTrolleyAssign_SendRequestCancelAMR";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            RequestNo = requestNo,
            TrolleyNo = trolleyNo,
            UserId = userId
        });
    }

    public async Task UpdateStatusAMR(string requestNo, string trolleyNo, string lastStatus)
    {
        string sql = "sp_Wms_Mobile_ManualTrolleyAssign_UpdateStatusAMR";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            RequestNo = requestNo,
            TrolleyNo = trolleyNo,
            LastStatus = lastStatus
        });
    }

    public async Task<List<ManualTrolleyDetailRequestDto>> GetListDetailRequestAMR(string requestNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_GetListDetailRequestAMR";
        return (await _dbExecutor.QueryListAsync<ManualTrolleyDetailRequestDto>(sp, new { RequestNo = requestNo })).ToList();
    }

    public async Task<Dictionary<string, object>> CaptureStatusAMR(string pickingNo)
    {
        string sp = "sp_Wms_Mobile_ManualTrolleyAssign_CaptureStatusAMR";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { PickingNo = pickingNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}
