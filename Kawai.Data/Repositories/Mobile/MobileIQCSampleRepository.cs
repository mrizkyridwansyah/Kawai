using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileIQCSampleRepository : IMobileIQCSampleRepository
{

    private readonly DbExecutor _dbExecutor;

    public MobileIQCSampleRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<IQCInspectionDto>> GetListSample(long receiptId)
    {
        string sp = "sp_Wms_Mobile_IQCSample_ListSample";
        return (await _dbExecutor.QueryListAsync<IQCInspectionDto>(sp, new { ReceiptId = receiptId })).ToList();
    }

    public async Task<IQCSampleDetailBarcodeDto> GetDataSampleBarcode(long receiptId, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_IQCSample_GetDataSample";
        return await _dbExecutor.QueryFirstOrDefaultAsync<IQCSampleDetailBarcodeDto>(sp, new { ReceiptId = receiptId, BarcodeNo = barcodeNo });
    }

    public async Task Save(MobileIQCSample payload, string userId)
    {
        string sql = "sp_Wms_Mobile_IQCSample_SaveSample";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sql, new
        {
            payload.ReceiptId,
            payload.BarcodeNo,
            payload.QtySample,
            UserId = userId
        });

        payload.InspectionId = newId;
    }

    public async Task<Dictionary<string, object>> Capture(long inspectionId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_IQCSample_Capture",
            param: new { InspectionId = inspectionId },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var detailBarcodes = (await multi.ReadAsync<dynamic>()).ToList();
                header.SampleDetails = detailBarcodes;
                return header;
            }
        );

        return new Dictionary<string, object>
        {
            { "IQC Sample", result }
        };
    }
}
