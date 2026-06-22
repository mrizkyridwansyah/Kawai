using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class AndonProductionRepository : IAndonProductionRepository
{

    private readonly DbExecutor _dbExecutor;

    public AndonProductionRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<AndonProductionDto>> GetHeaderInfo(string Line, string Model, string Scheduledate)
    {
        //string sp = "sp_Wms_Andon_WominRequest_GetList";
        //return (await _dbExecutor.QueryListAsync<AndonWominRequestDto>(sp)).ToList();
        string sp = "sp_Wms_Andon_Production_GetInfoHeader";
        return (await _dbExecutor.QueryListAsync<AndonProductionDto>(sp, new { LineCode = Line  ?? "" , ModelCls = Model ?? "" , ProductionDate = Scheduledate })).ToList();
    }

    public async Task<List<AndonProductionDto>> GetInfoSchedule(string Line, string Model, string Scheduledate)
    {
        //string sp = "sp_Wms_Andon_WominRequest_GetList";
        //return (await _dbExecutor.QueryListAsync<AndonWominRequestDto>(sp)).ToList();
        string sp = "sp_Wms_Andon_Production_GetInfoSchedule";
        return (await _dbExecutor.QueryListAsync<AndonProductionDto>(sp, new { LineCode = Line ?? "", ModelCls = Model ?? "", ProductionDate = Scheduledate })).ToList();
    }

    public async Task<List<AndonProductionDto>> GetInfoTrolley(string Line, string Model, string Scheduledate)
    {
        //string sp = "sp_Wms_Andon_WominRequest_GetList";
        //return (await _dbExecutor.QueryListAsync<AndonWominRequestDto>(sp)).ToList();
        string sp = "sp_Wms_Andon_Production_GetInfoTrolley";
        return (await _dbExecutor.QueryListAsync<AndonProductionDto>(sp, new { LineCode = Line ?? "", ModelCls = Model ?? "", ProductionDate = Scheduledate })).ToList();
    }



}
