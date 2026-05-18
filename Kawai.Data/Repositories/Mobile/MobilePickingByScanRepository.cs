using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Diagnostics;

namespace Kawai.Data.Repositories.Mobile;

public class MobilePickingByScanRepository : IMobilePickingByScanRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobilePickingByScanRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PickingByScanInstructionDto>> GetInstructionDDL(string keyword)
    {
        string sp = "sp_Wms_Mobile_PickingByScan_Instruction_DDL";

        return (await _dbExecutor.QueryListAsync<PickingByScanInstructionDto>(sp, new
        {
            Keyword = keyword ?? ""
        })).ToList();
    }

    public async Task<List<PickingByScanListDto>> GetListDetailShipping(string instructionNo, string keyword)
    {
        string sp = "sp_Wms_Mobile_PickingByScan_GetList";
        return (await _dbExecutor.QueryListAsync<PickingByScanListDto>(sp, new { InstructionNo = instructionNo, Keyword= keyword })).ToList();
    }

    public async Task<List<PickingByScanDetailDto>> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo)
    {
        string sp = "sp_Wms_Mobile_PickingByScan_GetDetail";
        return (await _dbExecutor.QueryListAsync<PickingByScanDetailDto>(sp, new { InstructionNo = instructionNo, BarcodeNo = barcodeNo, PartNo = partNo, SerialNo = serialNo })).ToList();
    }

    public async Task<PickingByScanDetailDto> GetDataBarcode(string barcodeNo, string instructionNo)
    {
        string sp = "sp_Wms_Mobile_PickingByScan_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PickingByScanDetailDto>(sp, new { BarcodeNo = barcodeNo, InstructionNo = instructionNo});
    }

    public async Task<bool> Save(MobilePickingByScanSubmit payload, string deviceId, string userId)
    {
        string sql = "sp_Wms_Mobile_PickingByScan_Submit";
        bool hasComplete = await _dbExecutor.QueryFirstOrDefaultAsync<bool>(sql, new
        {
            payload.InstructionNo,
            payload.BarcodeNo,
            DeviceID = deviceId,
            UserId = userId
        });

        return hasComplete;
    }

    public async Task<Dictionary<string, object>> Capture(string instructionNo, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_PickingByScan_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new {InstructionNo = instructionNo, BarcodeNo = barcodeNo});

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}
