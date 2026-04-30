using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileSupplySubConRepository : IMobileSupplySubconRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileSupplySubConRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<SupplySubconRequestNoDto>> GetRequestNoDDL(string keyword, string itemClass, string supplierCode)
    {
        string sp = "sp_Wms_Mobile_SupplySubcon_DDLRequestNo";
        return (await _dbExecutor.QueryListAsync<SupplySubconRequestNoDto>(sp, new
        {
            Keyword = keyword ?? "",
            ClassificationCode = String.IsNullOrEmpty(itemClass) ? "ALL" : itemClass,
            SupplierCode = String.IsNullOrEmpty(supplierCode) ? "ALL" : supplierCode
        })).ToList();
    }

    public async Task<SupplySubconDto> GetDataBarcode(string barcodeNo, string requestNo, string itemClass)
    {
        string sp = "sp_Wms_Mobile_SupplySubcon_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<SupplySubconDto>(sp, new { BarcodeNo = barcodeNo, RequestNo = requestNo, ClassificationCode = itemClass });
    }

    public async Task<List<SupplySubconDto>> GetListDetailMaterial(string requestno, string itemClass)
    {
        string sp = "sp_Wms_Mobile_SupplySubcon_GetListMaterial";
        return (await _dbExecutor.QueryListAsync<SupplySubconDto>(sp, new { RequestNo = requestno, ClassificationCode = itemClass })).ToList();
    }

    public async Task<List<StockDto>> GetListStock(string itemCode)
    {
        string sp = "sp_Wms_Mobile_SupplySubcon_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new { ItemCode = itemCode })).ToList();
    }

    public async Task Save(MobileSupplySubcon payload, string userId)
    {
        string sql = "sp_Wms_Mobile_SupplySubcon_Submit";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.BarcodeNo,
            payload.LotNo,
            payload.ItemCode,
            payload.RequestNoCode,
            ClassificationCode = payload.ItemClass,
            payload.Qty,
            UserID = userId
        });
    }


    public async Task<Dictionary<string, object>> Capture(string requestNo, string itemCode)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_SupplySubcon_Capture",
            param: new { RequestNo = requestNo, ItemCode = itemCode },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>())?.FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();

                if (header != null)
                    header.ScanDetails = details;

                return header;
            }
        );

        return new Dictionary<string, object>
        {
            { "Supply Subcon", result }
        };
    }
}
