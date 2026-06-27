using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;


namespace Kawai.Data.Repositories.Mobile
{
    public class MobileConsumptionUnscheduleRepository : IMobileConsumptionUnscheduleRepository
    {
        private readonly DbExecutor _dbExecutor;

        public MobileConsumptionUnscheduleRepository(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<ConsumpUnscheduleDto> GetDataBarcode(string barcodeNo)
        {
            string sp = "sp_Wms_Mobile_ConsumptionUnschedule_GetDataBarcode";
            return await _dbExecutor.QueryFirstOrDefaultAsync<ConsumpUnscheduleDto>(sp, new { BarcodeNo =barcodeNo });
        }

        public async Task Save(ConsumpUnscheduleSave payload, string userId)
        {
            string sql = "sp_Wms_Mobile_ConsumptionUnschedule_Submit";
            int i = await _dbExecutor.ExecuteAsync(sql, new
            {
                payload.BarcodeNo,
                payload.QtyInput,
                payload.Remarks,
                UserId = userId
            });
        }

        public async Task<Dictionary<string, object>> Capture(string barcodeNo)
        {
            string sp = "sp_Wms_Mobile_ConsumptionUnschedule_Capture";
            var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo });

            if (result == null)
                return new Dictionary<string, object>();

            return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
        }

    }
}
