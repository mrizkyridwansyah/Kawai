using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class AndonWominRequestRepository : IAndonWominRequestRepository
{

    private readonly DbExecutor _dbExecutor;

    public AndonWominRequestRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<AndonWominRequestDto>> GetListNSummary(string line, string Area)
    {
        //string sp = "sp_Wms_Andon_WominRequest_GetList";
        //return (await _dbExecutor.QueryListAsync<AndonWominRequestDto>(sp)).ToList();
        string sp = "sp_Wms_Andon_WominRequest_GetList";
        return (await _dbExecutor.QueryListAsync<AndonWominRequestDto>(sp, new { Line = line ?? "" , Area = Area ?? "" })).ToList();
    }

    public async Task<List<AndonWominRequestDetailDto>> GetListWomin(RequestParameter param)
    {
        string sp = "sp_Wms_Andon_WominRequest_GetListDetail";
        return (await _dbExecutor.QueryListAsync<AndonWominRequestDetailDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<AndonWominRequestDetailDto>> GetListWominByLine(RequestParameter param)
    {
        string sp = "sp_Wms_Andon_WominRequest_GetListDetailByArea";
        return (await _dbExecutor.QueryListAsync<AndonWominRequestDetailDto>(sp, param.ToQueryObject())).ToList();
    }



}
