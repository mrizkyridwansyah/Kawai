using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileMaterialStorageRepository : IMobileMaterialStorageRepository
{

    private readonly DbExecutor _dbExecutor;

    public MobileMaterialStorageRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<MaterialStorageSummaryDto>> GetSummaryStorage(string warehouseCode)
    {
        string sp = "sp_Wms_Mobile_MaterialStorage_GetSummaryStorage";
        return (await _dbExecutor.QueryListAsync<MaterialStorageSummaryDto>(sp, new { WarehouseCode = warehouseCode })).ToList();
    }

    public async Task<List<MaterialStorageDto>> GetListDetail(string warehouseCode, string lotNo, string itemCode)
    {
        string sp = "sp_Wms_Mobile_MaterialStorage_GetListDetail";
        return (await _dbExecutor.QueryListAsync<MaterialStorageDto>(sp, new { WarehouseCode = warehouseCode, LotNo = lotNo, ItemCode = itemCode })).ToList();
    }

    public async Task<MaterialStorageDto> GetDataBarcode(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_MaterialStorage_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<MaterialStorageDto>(sp, new { BarcodeNo = barcodeNo });
    }

    public async Task Save(MobileMaterialStorage payload, string userId)
    {
        string sql = "sp_Wms_Mobile_MaterialStorage_Save";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RefNo,
            payload.AddressCode,
            payload.BarcodeNo,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string refNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_MaterialStorage_Capture",
            param: new { RefNo = refNo },
            async multi =>
            {
                var stocks = (await multi.ReadAsync<StockMasterDto>()).ToList();
                var stockDetail = (await multi.ReadAsync<StockDetailDto>()).ToList();
                foreach (var master in stocks)
                {
                    master.StockDetails = stockDetail
                    .Where(detail =>
                        detail.RefNo == master.RefNo &&
                        detail.WarehouseCode == master.WarehouseCode &&
                        detail.AreaCode == master.AreaCode &&
                        detail.ItemCode == master.ItemCode &&
                        detail.LotNo == master.LotNo
                    ).ToList();
                }
                return stocks;
            }
        );

        return new Dictionary<string, object>
        {
            { "Material Storage", result }
        };
    }
}
