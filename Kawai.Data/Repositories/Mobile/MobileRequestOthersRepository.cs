using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

class MobileRequestOthersRepository : IMobileRequestOthersRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileRequestOthersRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<RequestOthersRequestNoDto>> GetRequestNoDDL(string keyword, string linecode, string requestno)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_DDL";
        return (await _dbExecutor.QueryListAsync<RequestOthersRequestNoDto>(sp, new
        {
            Keyword = keyword ?? "",
            LineCode = String.IsNullOrEmpty(linecode) ? "ALL" : linecode,
            RequestNo = String.IsNullOrEmpty(requestno) ? "ALL" : requestno
        })).ToList();
    }
    public async Task<List<RequestOthersDto>> GetListDetailMaterial(string requestno, string linecode)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_GetListMaterial";
        return (await _dbExecutor.QueryListAsync<RequestOthersDto>(sp, new { RequestNo = requestno, LineCode = linecode })).ToList();
    }
    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string warehouseCode)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode })).ToList();
    }
    public async Task<List<ManufactureLineDto>> GetProcessDDL(string keyword)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_ProcessDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? ""})).ToList();
    }
    public async Task<List<RequestOthersDetailDto>> GetListDetail(string linecode, string requestno,   string itemCode)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_GetListDetail";
        return (await _dbExecutor.QueryListAsync<RequestOthersDetailDto>(sp, new { Linecode = linecode, RequestNo = requestno,  ItemCode = itemCode })).ToList();
    }
    public async Task<RequestOthersDto> GetDataBarcode(string barcodeNo, string claimno, string linecode)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<RequestOthersDto>(sp, new { BarcodeNo = barcodeNo, RequestNo = requestno, LineCode = linecode });
    }
    public async Task<bool> Save(MobileRequestOthersSubmit payload, string userId)
    {
        string sql = "sp_Wms_Mobile_RequestOthers_Submit";
        bool hasCompelete = await _dbExecutor.QueryFirstOrDefaultAsync<bool>(sql, new
        {
            payload.WarehouseCode,
            payload.BarcodeNo,
            payload.LineCode,
            payload.LotNo,
            payload.ItemCode,
            payload.RequestNo,
            payload.Qty,
            UserId = userId
        });

        return hasCompelete;
    }
    public async Task<Dictionary<string, object>> Capture(string barcodeNo, string requestno, string linecode)
    {
        string sp = "sp_Wms_Mobile_RequestOthers_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo, RequestNo = requestno, LineCode = linecode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }


   
}

