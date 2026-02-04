using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

class MobileSupplyScanRequestRepository : IMobileSupplyScanRequestRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileSupplyScanRequestRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string linecode)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequestNo_DDL";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestNoDto>(sp, new
        {
            Keyword = keyword ?? "",
            LineCode = String.IsNullOrEmpty(linecode) ? "ALL" : linecode
        })).ToList();
    }
    public async Task<List<SupplyScanRequestDto>> GetListDetailMaterial(string requestno)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetListMaterial";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestDto>(sp, new { RequestNo = requestno })).ToList();
    }

    public async Task<List<SupplyScanRequestDetailDto>> GetListDetail(string warehouseCode, string requestNo, string itemCode)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetListDetail";
        return (await _dbExecutor.QueryListAsync<SupplyScanRequestDetailDto>(sp, new { WarehouseCode =   warehouseCode ,RequestNo = requestNo , ItemCode = itemCode })).ToList();
    }


    //public async Task<List<SupplyScanRequestDto>> GetListDetail(string lotNo, string itemCode)
    //{
    //    string sp = "sp_Wms_Mobile_StockInventoryUpdate_GetListDetail";
    //    return (await _dbExecutor.QueryListAsync<StockInventoryUpdateDto>(sp, new { LotNo = lotNo, ItemCode = itemCode })).ToList();
    //}

    public async Task<SupplyScanRequestDto> GetDataBarcode(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<SupplyScanRequestDto>(sp, new { BarcodeNo = barcodeNo });
    }

    public async Task Save(MobilSupplyScanRequestSubmit payload, string userId)
    {
        string sql = "sp_Wms_Mobile_SupplyScanRequest_Submit";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.WarehouseCode  ,
            payload.BarcodeNo  ,
            payload.LineCode  ,
            payload.LotNo  ,
            payload.ItemCode  ,
            payload.RequestNoCode,
            payload.Qty  ,
            UserId = userId
        });
    }


    public async Task<Dictionary<string, object>> Capture(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_SupplyScanRequest_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }


}
 
