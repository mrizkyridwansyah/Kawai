using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Models;

namespace Kawai.Data.Repositories.Mobile;

public class MobileMovingTrolleyRepository: IMobileMovingTrolleyRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileMovingTrolleyRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<MovingTrolleyDto>> GetDataTrolley(string trolleyNo)
    {
        string sp = "sp_Wms_MovingTrolley_GetDataTrolley";
        return (await _dbExecutor.QueryListAsync<MovingTrolleyDto>(sp, new { TrolleyNo = trolleyNo })).ToList();
    }

    public async Task<StopPointDto> GetDataStopPoint(string stopPoint)
    {
        string sp = "sp_Wms_MovingTrolley_GetDataStopPoint";
        return await _dbExecutor.QueryFirstOrDefaultAsync<StopPointDto>(sp, new { StopPoint = stopPoint });
    }

    public async Task Save(MobileMovingTrolley payload, string userId)
    {
        string sql = "sp_Wms_MovingTrolley_Submit";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RequestNo,
            payload.TrolleyNo,
            payload.StopPoint,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string trolleyNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_MaterialStorage_Capture",
            param: new { RefNo = trolleyNo },
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
            { "Moving Trolley", result }
        };
    }

}
