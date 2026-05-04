using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Robot;
using System.Data.Common;
using System.Data;

namespace Kawai.Data.Repositories.Robot;

public class RobotRepository : IRobotRepository
{

    private readonly DbExecutor _dbExecutor;

    public RobotRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<string> GetListData()
    {
        string sp = "sp_Wms_SendRobot";

        return await _dbExecutor.QueryFirstOrDefaultAsync<string>(
            sp,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task SetTrolleyAsync(SetTrolleyRequest payload)
    {
        string sql = "sp_Wms_Robot_SetTrolley_Update";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RequestID,
            payload.TrolleyNo
        });
    }

    public async Task CompleteStatusAsync(CompleteStatusRequest payload)
    {
        string sql = "sp_Wms_Robot_CompleteStatus";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.RequestID,
            payload.TrolleyNo,
            payload.StopPoint,
            payload.Status
        });
    }

    public async Task EmptyTrolleyAsync(EmptyTrolleyRequest payload)
    {
        string sql = "sp_Wms_Robot_EmptyTrolley";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.TrolleyNo
        });
    }
    public async Task<Dictionary<string, object>> CaptureSetTrolley(SetTrolleyRequest payload)
    {
        string sp = "sp_Wms_Robot_SetTrolley_Capture";
        var param = new
        {
            TrolleyNo = payload.TrolleyNo,
            RequestID = payload.RequestID
        };

        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, param);

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<Dictionary<string, object>> CaptureCompleteStatus(CompleteStatusRequest payload)
    {
        string sp = "sp_Wms_Robot_CompleteStatus_Capture";
        var param = new
        {
            RequestID = payload.RequestID,
            TrolleyNo = payload.TrolleyNo,
            StopPoint = payload.StopPoint
        };

        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, param);
        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<Dictionary<string, object>> CaptureEmptyTrolley(EmptyTrolleyRequest payload)
    {
        string sp = "sp_Wms_Robot_EmptyTrolley_Capture";
        var param = new
        {
            TrolleyNo = payload.TrolleyNo
        };
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, param);
        if (result == null)
            return new Dictionary<string, object>();
        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}