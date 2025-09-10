using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IQualityCheckRepository
{
    Task<List<QualityCheckDto>> GetAll(RequestParameter param);
    Task<List<QualityCheckDto>> DDLDNNoBySupplierDateFrom(string keyword, string supplier, string receiptdatefrom, string receiptdateto);
    Task<QualityCheckDto> GetData(string Id);
    Task Confirm(QualityCheck qualitycheck, string userid);
    Task<Dictionary<string, object>> Capture(string id);
   

}
