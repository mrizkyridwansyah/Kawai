using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

class MobileSupplyScanRequestRepository : IMobileSupplyScanRequestRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileSupplyScanRequestRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string linecode, string warehouse)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequestNo_DDL";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestNoDto>(sp, new
        {
            Keyword = keyword ?? "",
            LineCode = String.IsNullOrEmpty(linecode) ? "ALL" : linecode,
            WarehouseCode = String.IsNullOrEmpty(warehouse) ? "ALL" : warehouse
        })).ToList();
    }
    public async Task<List<SupplyScanRequestDto>> GetListDetailMaterial(string requestno, string itemClass)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetListMaterial";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestDto>(sp, new { RequestNo = requestno, ItemClass = itemClass })).ToList();
    }

    public async Task<List<WarehouseDto>> GetWarehouseDDL(string keyword, string userId)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequestNo_WarehouseDDL";
        return (await _dbExecutor.QueryListAsync<WarehouseDto>(sp, new { Keyword = keyword ?? "", UserId = userId })).ToList();
    }
    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string warehouseCode)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequestNo_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode })).ToList();
    }

    public async Task<List<SupplyScanRequestDetailDto>> GetListDetail(string warehouseCode, string requestNo, string itemCode)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetListDetail";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestDetailDto>(sp, new { WarehouseCode = warehouseCode, RequestNo = requestNo, ItemCode = itemCode })).ToList();
    }

    public async Task<SupplyScanRequestDto> GetDataBarcode(string barcodeNo, string requestNo, string itemClass)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<SupplyScanRequestDto>(sp, new { BarcodeNo = barcodeNo, RequestNo = requestNo, ItemClass = itemClass });
    }

    public async Task<bool> Save(MobilSupplyScanRequestSubmit payload, string userId)
    {
        string sql = "sp_Wms_Mobile_SupplyScanRequest_Submit";
        bool hasCompelete = await _dbExecutor.QueryFirstOrDefaultAsync<bool>(sql, new
        {
            payload.WarehouseCode,
            payload.BarcodeNo,
            payload.LineCode,
            payload.LotNo,
            payload.ItemCode,
            payload.RequestNoCode,
            payload.ItemClass,
            payload.Qty,
            UserId = userId
        });

        return hasCompelete;
    }


    public async Task<Dictionary<string, object>> Capture(string barcodeNo, string pickingNo, string itemClass)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo, PickingNo = pickingNo, ItemClass = itemClass });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }


    public async Task<SupplyScanRequestAMRDto> GetRequestAMR(string requestNo)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetRequestAMR";
        return await _dbExecutor.QueryFirstOrDefaultAsync<SupplyScanRequestAMRDto>(sp, new { RequestNo = requestNo });
    }

    public async Task UpdateStatusAMR(string requestNo, string lastStatus)
    {
        string sql = "sp_Wms_Mobile_SupplyScanRequest_UpdateStatusAMR";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            RequestNo = requestNo,
            LastStatus = lastStatus
        });
    }

    public async Task SendRequestCancelAMR(string requestNo, string userId)
    {
        string sql = "sp_Wms_Mobile_SupplyScanRequest_SendRequestAMR";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            RequestNo = requestNo,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> CaptureStatusAMR(string requestNo)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_CaptureStatusAMR";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { RequestNo = requestNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}

