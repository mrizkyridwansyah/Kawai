using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;


namespace Kawai.Data.Repositories.Mobile
{
    public class MobileProductionResultScanRepository : IMobileProductionResultScanRepository
    {
        private readonly DbExecutor _dbExecutor;

        public MobileProductionResultScanRepository(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<MobileProductionResultScanDto> GetDataBarcode(string barcodeNo)
        {
            string sp = "sp_Wms_Mobile_ProductionResultScan_GetDataBarcode";
            return await _dbExecutor.QueryFirstOrDefaultAsync<MobileProductionResultScanDto>(sp, new { BarcodeNo =barcodeNo });
        }

        public async Task Save(MobileProductionResultSubmit payload, string userId)
        {
            string sql = "sp_Wms_Mobile_ProductionResultScan_Submit";
            int i = await _dbExecutor.ExecuteAsync(sql, new
            {
                payload.BarcodeNo,
                payload.LotNo,
                payload.ItemCode,
                payload.LineCode,
                payload.Qty,
                UserId = userId
            });
        }

        public async Task<Dictionary<string, object>> Capture(string barcodeNo)
        {
            string sp = "sp_Wms_Mobile_ProductionResultScan_Capture";
            var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo });

            if (result == null)
                return new Dictionary<string, object>();

            return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
        }

    }
}
