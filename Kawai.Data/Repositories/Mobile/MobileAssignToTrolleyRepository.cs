using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileAssignToTrolleyRepository : IMobileAssignToTrolleyRepository
{

    private readonly DbExecutor _dbExecutor;

    public MobileAssignToTrolleyRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<AssignToTrolleyDto> GetDataTrolley(string trolleyNo)
    {
        string sp = "sp_Wms_Mobile_AssignToTrolleyByBarcode_GetDataTrolley";
        return await _dbExecutor.QueryFirstOrDefaultAsync<AssignToTrolleyDto>(sp, new { TrolleyNo = trolleyNo });
    }

    public async Task Save(MobileAssignToTrolley payload, string userId)
    {
        string sql = "sp_Wms_Mobile_AssignToTrolleyByBarcode_Scan";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.BarcodeNo,
            payload.TrolleyNo,
            UserId = userId
        });
    }
}
