using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IQualityCheckRepository
{
    Task<List<QualityCheckDto>> GetAll(RequestParameter param);
    Task<List<QualityCheckDto>> DDLDNNoBySupplierDateFrom(string keyword, string supplier, string receiptdatefrom, string receiptdateto);
    Task<QualityCheckResultDto> GetData(long inspectionid);
    Task Save(QualityCheckResult result, string userid);
    Task Confirm(QualityCheckConfirm payload, string userid);
    Task<Dictionary<string, object>> Capture(long id);
   
    Task<List<QualityCheckReportDto>> PrintReportNG(long receiptId);

}
