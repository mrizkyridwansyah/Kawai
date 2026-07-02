using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using System.Data;

namespace Kawai.Data.Repositories.Mobile
{
    public class MobileBarcodeTraceabilityRepository : IMobileBarcodeTraceabilityRepository
    {
        private readonly DbExecutor _dbExecutor;

        public MobileBarcodeTraceabilityRepository(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }



        public async Task<BarcodeTraceabilityDto> GetDataBarcode(string barcodeNo)
        {
            string sp = "sp_Wms_Mobile_BarcodeTraceability_GetDataBarcode";
            

            var result= await _dbExecutor.QueryMultipleAsync(
                sp,
                new { BarcodeNo = barcodeNo },
                async multi =>
                {
                    var result = new BarcodeTraceabilityDto();

                    // Result Set 1 : Header
                    result.Header = await multi.ReadFirstOrDefaultAsync<BarcodeTraceabilityHeaderDto>();

                    // Result Set 2 : Detail
                    result.Details = (await multi.ReadAsync<BarcodeTraceabilityListDto>()).ToList();

                    return result;
                });

            return result;
        }
    }

}
