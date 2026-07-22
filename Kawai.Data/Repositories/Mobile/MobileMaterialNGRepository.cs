using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileMaterialNGRepository : IMobileMaterialNGRepository
{

    private readonly DbExecutor _dbExecutor;

    public MobileMaterialNGRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<MaterialNGDto> GetDataNGBarcode(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_MaterialNG_GetDataNG";
        return await _dbExecutor.QueryFirstOrDefaultAsync<MaterialNGDto>(sp, new { BarcodeNo = barcodeNo });
    }

    public async Task Save(MobileMaterialNG  ng, string userId)
    {
        string sql = "sp_Wms_Mobile_MaterialNG_SaveNG";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sql, new
        {
            ng.BarcodeNo,
            ng.QtyNG,
            ng.NGCode,
            UserId = userId
        });

        ng.InspectionId = newId;
    }

    public async Task<Dictionary<string, object>> Capture(long inspectionId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Mobile_MaterialNG_Capture",
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
            { "Material NG", result }
        };
    }
}
