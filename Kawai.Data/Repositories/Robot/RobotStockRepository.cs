using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Data.Repositories.Robot;

public class RobotStockRepository: IRobotStockRepository
{

    private readonly DbExecutor _dbExecutor;

    public RobotStockRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task MoveTrolley(RobotMovingTrolley payload, string robotCode)
    {
        string sqlHeader = "sp_Wms_Robot_MoveTrollery";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            payload.TrolleyCode,
            RobotCode = robotCode,
        });
    }

    public async Task<Dictionary<string, object>> CaptureDataGrouping(string refNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Robot_CaptureStock",
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
            { "Trolley No: ", refNo },
            { "Stock", result }
        };
    }
}
