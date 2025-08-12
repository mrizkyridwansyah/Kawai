using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class DeliveryPlaceRepository : IDeliveryPlaceRepository
{
    private readonly DbExecutor _dbExecutor;

    public DeliveryPlaceRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<DeliveryPlaceDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_DeliveryPlace_List";
        return (await _dbExecutor.QueryListAsync<DeliveryPlaceDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<DeliveryPlaceDto> GetData(string trade_code, string location_code)
    {
        string sp = "sp_Wms_DeliveryPlace_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<DeliveryPlaceDto>(sp, new { Trade_Code = trade_code, Location_Code = location_code });
    }

    public async Task<List<DeliveryPlaceDto>> GetDDL(string trade_code, string keyword)
    {
        string sp = "sp_Wms_DeliveryPlace_DDL";
        return (await _dbExecutor.QueryListAsync<DeliveryPlaceDto>(sp, new { Trade_Code = trade_code, Keyword = keyword ?? "" })).ToList();
    }

    

    public async Task Create(DeliveryPlace dp, string userId)
    {
        string sql = @"sp_Wms_DeliveryPlace_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            dp.Trade_Code,
            dp.Location_Code,
            dp.Location_Name, 
            RegisterBy = userId
 
        });
    }

    public async Task Update(DeliveryPlace dp,  string userId)
    {
        string sql = @"sp_Wms_DeliveryPlace_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            dp.Trade_Code,
            dp.Location_Code,
            dp.Location_Name,
            UpdateBy = userId
        });
    }

    public async Task Remove(string trade_code, string location_code, string userId)
    {
        string sql = "sp_Wms_DeliveryPlace_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { Trade_Code = trade_code, Location_Code = location_code });
    }

    public async Task<Dictionary<string, object>> Capture(string tc, string location_code)
    {
        string sp = "sp_Wms_DeliveryPlace_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Trade_Code = tc, Location_Code = location_code });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
