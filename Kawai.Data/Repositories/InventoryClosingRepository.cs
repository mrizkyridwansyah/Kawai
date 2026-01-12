using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;
using System.Data;
using Dapper;

namespace Kawai.Data.Repositories;

public class InventoryClosingRepository : IInventoryClosingRepository
{

    private readonly DbExecutor _dbExecutor;

    public InventoryClosingRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }


    public async Task<InventoryClosingDto> GetCurrent()
    {

        return await _dbExecutor.QueryFirstOrDefaultAsync<InventoryClosingDto>(
            "sp_Wms_InventoryClosing_GetLastClosing"
        );
    }

    public async Task ProcessClosing(int ivtYear, int ivtMonth, string userId)
    {
        await _dbExecutor.QueryListAsync<object>(
            "sp_WMS_Closing",
            new
            {
                IvtYear = ivtYear,
                IvtMonth = ivtMonth,
                UserId = userId
            }
        );
    }







}
