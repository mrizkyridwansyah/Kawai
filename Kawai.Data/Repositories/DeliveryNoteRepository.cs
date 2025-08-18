using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class DeliveryNoteRepository : IDeliveryNoteRepository
{

    private readonly DbExecutor _dbExecutor;

    public DeliveryNoteRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<DeliveryNoteDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_DeliveryNote_List";
        return (await _dbExecutor.QueryListAsync<DeliveryNoteDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<DeliveryNoteDto> GetDetail(string dnNumber)
    {
        string sp = "sp_Wms_DeliveryNote_Detail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<DeliveryNoteDto>(sp, new { DNNumber = dnNumber });
    }

    public async Task<List<DeliveryNoteDetailDto>> GetListDetail(string dnNumber)
    {
        string sp = "sp_Wms_DeliveryNote_ListDetail";
        return (await _dbExecutor.QueryListAsync<DeliveryNoteDetailDto>(sp, new { DNNumber = dnNumber })).ToList();
    }
}
