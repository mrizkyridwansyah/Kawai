using Kawai.Data.SqlConnections;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class QualityCheckRepository : IQualityCheckRepository
{
    private readonly DbExecutor _dbExecutor;

    public QualityCheckRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<QualityCheckDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_IQCResult_List";
        var paramSupplier = param.GetParam("SupplierCode");
        var paramStatus = param.GetParam("StatusInspection");
        var paramSource = param.GetParam("Source");
        var paramDateFrom = param.GetParam("PeriodFrom");
        var paramDateUntil = param.GetParam("PeriodUntil");
        var paramReceiptId = param.GetParam("ReceiptId");

        return (await _dbExecutor.QueryListAsync<QualityCheckDto>(sp, new
        {
            SupplierCode = paramSupplier,
            Source = paramSource,
            StatusInspection = paramStatus,
            PeriodFrom = paramDateFrom,
            PeriodUntil = paramDateUntil,
            ReceiptId = String.IsNullOrEmpty(paramReceiptId) ? 0 : paramReceiptId.ToInt32(),
        })).ToList();
    }

    public async Task<QualityCheckResultDto> GetData(long inspectionId)
    {
        string sp = "sp_Wms_IQCResult_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<QualityCheckResultDto>(sp, new { InspectionId = inspectionId });
    }

    public async Task Save(QualityCheckResult payload, string userId)
    {
        string sql = @"sp_Wms_IQCResult_SaveResult";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.InspectionId,
            payload.QtyNG,
            payload.Remarks,
            payload.AttachmentName,
            UserId = userId
        });
    }

    public async Task Confirm(QualityCheckConfirm payload, string userId)
    {
        string sql = @"sp_Wms_IQCResult_Confirm";
        int i = await _dbExecutor.ExecuteNonTransactionAsync(sql, new
        {
            payload.InspectionId,
            payload.InspectionResult,
            payload.TypeHold,
            UserId = userId
        });
    }

    public async Task ConfirmSA(QualityCheckConfirm payload, string userId)
    {
        string sql = @"sp_Wms_IQCResult_ConfirmSA";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.InspectionId,
            payload.InspectionResult,
            UserId = userId
        });
    }

    public async Task ApprovalSA(QualityCheckConfirmSA payload, string userId)
    {
        string sql = @"sp_Wms_IQCResult_ApprovalSA";
        int i = await _dbExecutor.ExecuteNonTransactionAsync(sql, new
        {
            payload.InspectionId,
            payload.InspectionResult,
            payload.TypeHold,
            payload.RemarksSA,
            UserId = userId
        });
    }

    public async Task ApprovalSAUnapprove(QualityCheckConfirmSA payload, string userId)
    {
        string sql = @"sp_Wms_IQCResult_ApprovalSAFromUnapprove";
        int i = await _dbExecutor.ExecuteNonTransactionAsync(sql, new
        {
            payload.InspectionId,
            payload.InspectionResult,
            payload.TotalGoodQty,
            payload.TypeHold,
            payload.RemarksSA,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(long id)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_IQCResult_Capture",
            param: new { Id = id },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
                header.SampleDetails = details;
                return header;

            }
        );

        return new Dictionary<string, object>
        {
            { "IQC", result },
        };
    }

    public async Task<List<QualityCheckDto>> DDLDNNoBySupplierDateFrom(string keyword, string supplier, string receiptdatefrom, string receiptdateto)
    {
        string sp = "sp_Wms_QualityCheck_DDLDNNoBySupplierDateFrom";
        return (await _dbExecutor.QueryListAsync<QualityCheckDto>(sp, new
        {
            Keyword = keyword ?? "",
            SupplierCode = !String.IsNullOrEmpty(supplier) ? supplier : "ALL",
            ReceiptDateFrom = !String.IsNullOrEmpty(receiptdatefrom) ? receiptdatefrom : "ALL",
            ReceiptDateTo = !String.IsNullOrEmpty(receiptdateto) ? receiptdateto : "ALL"
        })).ToList();
    }

    public async Task<List<QualityCheckReportDto>> PrintReportNG(long receiptId)
    {
        string sp = "sp_Wms_QualityCheck_PrintReportNG";
        return (await _dbExecutor.QueryListAsync<QualityCheckReportDto>(sp, new
        {
            ReceiptId = receiptId,
        })).ToList();
    }
}
