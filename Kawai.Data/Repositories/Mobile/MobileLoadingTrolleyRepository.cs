using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileLoadingTrolleyRepository : IMobileLoadingTrolleyRepository
{

    private readonly DbExecutor _dbExecutor;

    public MobileLoadingTrolleyRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<LoadingTrolleyDto> GetDataTrolley(string trolleyNo)
    {
        string sp = "sp_Wms_Mobile_LoadingTrolley_GetDataTrolley";
        return await _dbExecutor.QueryFirstOrDefaultAsync<LoadingTrolleyDto>(sp, new { TrolleyNo = trolleyNo });
    }

    public async Task<List<StockDto>> GetDataBarcode(string trolleyNo, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_LoadingTrolley_GetDataBarcode";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new { TrolleyNo = trolleyNo, BarcodeNo = barcodeNo })).ToList();
    }

    public async Task Save(MobileLoadingTrolley payload, string userId)
    {
        string sql = "sp_Wms_Mobile_LoadingTrolley_Save";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.TrolleyNo,
            payload.RequestDetailId,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string refNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_AssignStorage_Capture",
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
            { "Loading Trolley", result }
        };
    }
}
