using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

class MobileProductionClaimNGRepository : IMobileProductionClaimNGRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileProductionClaimNGRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<ProductionClaimNGClaimNoDto>> GetClaimNoDDL(string keyword, string linecode, string claimno)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_DDL";
        return (await _dbExecutor.QueryListAsync<ProductionClaimNGClaimNoDto>(sp, new
        {
            Keyword = keyword ?? "",
            LineCode = String.IsNullOrEmpty(linecode) ? "ALL" : linecode,
            ClaimNo = String.IsNullOrEmpty(claimno) ? "ALL" : claimno
        })).ToList();
    }
    public async Task<List<ProductionClaimNGDto>> GetListDetailMaterial(string claimno, string linecode)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_GetListMaterial";
        return (await _dbExecutor.QueryListAsync<ProductionClaimNGDto>(sp, new { ClaimNo = claimno, LineCode = linecode })).ToList();
    }
    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string warehouseCode)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode })).ToList();
    }
    public async Task<List<ManufactureLineDto>> GetProcessDDL(string keyword)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_ProcessDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? ""})).ToList();
    }
    public async Task<List<ProductionClaimNGDetailDto>> GetListDetail(string linecode, string claimno, string pickingno, string itemCode)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_GetListDetail";
        return (await _dbExecutor.QueryListAsync<ProductionClaimNGDetailDto>(sp, new { Linecode = linecode, ClaimNo = claimno, PickingNo = pickingno, ItemCode = itemCode })).ToList();
    }
    public async Task<ProductionClaimNGDto> GetDataBarcode(string barcodeNo, string claimno, string linecode)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ProductionClaimNGDto>(sp, new { BarcodeNo = barcodeNo, ClaimNo = claimno, LineCode = linecode });
    }
    public async Task<bool> Save(MobilProductionClaimNGSubmit payload, string userId)
    {
        string sql = "sp_Wms_Mobile_ProductionClaimNG_Submit";
        bool hasCompelete = await _dbExecutor.QueryFirstOrDefaultAsync<bool>(sql, new
        {
            payload.WarehouseCode,
            payload.BarcodeNo,
            payload.LineCode,
            payload.LotNo,
            payload.ItemCode,
            payload.ClaimNo,
            payload.PickingNo,
            payload.Qty,
            UserId = userId
        });

        return hasCompelete;
    }
    public async Task<Dictionary<string, object>> Capture(string barcodeNo, string claimno, string linecode)
    {
        string sp = "sp_Wms_Mobile_ProductionClaimNG_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo, ClaimNo = claimno, LineCode = linecode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }


   
}

