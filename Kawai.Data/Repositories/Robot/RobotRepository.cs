using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Robot;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models.Robot;
using System.Data;

namespace Kawai.Data.Repositories.Robot;

public class RobotRepository : IRobotRepository
{

    private readonly DbExecutor _dbExecutor;

    public RobotRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<SupplyRequestDto>> GetListData(string reqId)
    {
        string sp = "sp_Wms_Robot_GetDataRequest";
        return (await _dbExecutor.QueryListAsync<SupplyRequestDto>(sp, new { RequestSendID = reqId })).ToList();
    }

    public async Task<SupplyRequestCompleteDto> GetRequestData(string reqId, string stopPoint)
    {
        string sp = "sp_Wms_Robot_GetRequestData";
        return await _dbExecutor.QueryFirstOrDefaultAsync<SupplyRequestCompleteDto>(sp, new { RequestSendID = reqId, StopPoint = stopPoint });
    }

    public async Task SetTrolleyAsync(SetTrolleyRequest payload)
    {
        string sql = "sp_Wms_Robot_SetTrolley";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RequestID,
            payload.TrolleyNo,
            payload.Status,
        });
    }

    public async Task MoveTrolley(MovingTrolleyRequest payload)
    {
        string sqlHeader = "sp_Wms_Robot_MoveTrolley";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            RefNo = payload.TrolleyNo,
            payload.AddressCode,
            payload.StopPointCode,
            //payload.RobotCode,
        });
    }

    public async Task EmptyTrolleyAsync(EmptyTrolley payload)
    {
        string sql = "sp_Wms_Robot_EmptyTrolley";
        var result = await _dbExecutor.QuerySingleOrDefaultAsync<EmptyTrolley>(sql, new
        {
            RefNo = payload.TrolleyNo
        });

        if (result != null)
        {
            payload.NewRefNo = result.NewRefNo;
            payload.PickingNo = result.PickingNo;
        }
    }

    public async Task<Dictionary<string, object>> CaptureSetTrolley(SetTrolleyRequest payload)
    {
        string sp = "sp_Wms_Robot_SetTrolley_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { payload.RequestID });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<Dictionary<string, object>> CaptureStockTrolley(string trolleyNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Robot_CaptureStockTrolley",
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
            { "Trolley No: ", trolleyNo },
            { "Stock", result }
        };
    }
}