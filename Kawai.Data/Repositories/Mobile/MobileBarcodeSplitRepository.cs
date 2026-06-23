using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileBarcodeSplitRepository: IMobileBarcodeSplitRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileBarcodeSplitRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<BarcodeSplitDto> GetDataBarcode(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_BarcodeSplit_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<BarcodeSplitDto>(sp, new { BarcodeNo = barcodeNo });
    }

    public async Task<List<BarcodeSplitHistoryDto>> GetHistorySplit(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_BarcodeSplit_GetHistorySplit";
        return (await _dbExecutor.QueryListAsync<BarcodeSplitHistoryDto>(sp, new { BarcodeNo = barcodeNo })).ToList();
    }

    public async Task Save(MobileBarcodeSplit payload, string userId)
    {
        string sql = "sp_Wms_Mobile_BarcodeSplit_Save";

        string barcodeNoNew =  await _dbExecutor.QuerySingleOrDefaultAsync<string>(sql, new
        {
            payload.BarcodeNo,
            payload.QtySplit,
            UserId = userId
        });

        payload.BarcodeNoNew = barcodeNoNew;
    }

    public async Task<Dictionary<string, object>> Capture(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_BarcodeSplit_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);

    }
}
