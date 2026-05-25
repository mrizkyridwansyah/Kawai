using Dapper;
using Kawai.Data.SqlConnections;
using Kawai.Domain;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace Kawai.Data.Repositories.Mobile;

public class MobileLoadingConfirmationRepository : IMobileLoadingConfirmationRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileLoadingConfirmationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    private DataTable GenerateEvidenceFileTable(List<LoadingConfirmationEvidenceFileQueue> files)
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("FileName", typeof(string));
        dt.Columns.Add("FilePath", typeof(string));
        dt.Columns.Add("Remarks", typeof(string));

        foreach (var item in files)
        {
            dt.Rows.Add(
                item.FileName,
                item.FilePath,
                item.Remarks
            );
        }

        return dt;
    }

    private DataTable GenerateExistingEvidenceFileTable(List<ExistingEvidenceFile> files)
    {
        var dt = new DataTable();

        dt.Columns.Add("FileName");
        dt.Columns.Add("FilePath");
        dt.Columns.Add("Remarks");

        if (files == null)
            return dt;

        foreach (var item in files)
        {
            dt.Rows.Add(
                item.FileName,
                item.FilePath,
                item.Remarks
            );
        }

        return dt;
    }

    public async Task<List<LoadingConfirmationInstructionDto>> GetInstructionDDL(string keyword)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_Instruction_DDL";

        return (await _dbExecutor.QueryListAsync<LoadingConfirmationInstructionDto>(sp, new
        {
            Keyword = keyword ?? ""
        })).ToList();
    }

    public async Task<List<LoadingConfirmationListDto>> GetListDetailShipping(string instructionNo, string keyword)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_GetList";
        return (await _dbExecutor.QueryListAsync<LoadingConfirmationListDto>(sp, new { InstructionNo = instructionNo, Keyword = keyword })).ToList();
    }

    public async Task<List<LoadingConfirmationDetailDto>> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_GetDetail";
        return (await _dbExecutor.QueryListAsync<LoadingConfirmationDetailDto>(sp, new { InstructionNo = instructionNo, BarcodeNo = barcodeNo, PartNo = partNo, SerialNo = serialNo })).ToList();
    }

    public async Task<LoadingConfirmationDetailDto> GetDataBarcode(string barcodeNo, string instructionNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<LoadingConfirmationDetailDto>(sp, new { BarcodeNo = barcodeNo, InstructionNo = instructionNo });
    }

    public async Task<bool> Save(MobileLoadingConfirmationSubmit payload, string deviceId, string userId)
    {
        string sql = "sp_Wms_Mobile_LoadingConfirmation_Submit";
        bool hasComplete = await _dbExecutor.QueryFirstOrDefaultAsync<bool>(sql, new
        {
            payload.InstructionNo,
            payload.BarcodeNo,
            DeviceID = deviceId,
            UserId = userId
        });

        return hasComplete;
    }

    public async Task<Dictionary<string, object>> Capture(string instructionNo, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { InstructionNo = instructionNo, BarcodeNo = barcodeNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<bool> SubmitEvidenceBefore(MobileLoadingConfirmationEvidenceBeforeQueueSubmit payload, string userId)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_EvidenceBefore_Submit";
        var fileTable = GenerateEvidenceFileTable(payload.Files);
        var existingFileTable = GenerateExistingEvidenceFileTable(payload.ExistingFiles);
        var parameters = new DynamicParameters();

        parameters.Add("@InstructionNo", payload.InstructionNo);
        parameters.Add("@ContainerNo", payload.ContainerNo);
        parameters.Add("@VehicleNo", payload.VehicleNo);
        parameters.Add("@SealNo", payload.SealNo);
        parameters.Add("@Remark", payload.Remark);
        parameters.Add("@DriverName", payload.DriverName);
        parameters.Add("@TransportVendor", payload.TransportVendor);
        parameters.Add("@Latitude", payload.Latitude);
        parameters.Add("@Longitude", payload.Longitude);
        parameters.Add("@RevisionReason", payload.RevisionReason);
        parameters.Add("@UserID", userId);
        parameters.Add("@Files", fileTable.AsTableValuedParameter("dbo.tvp_LoadingConfirmationEvidenceFile"));
        parameters.Add("@ExistingFiles", existingFileTable.AsTableValuedParameter("dbo.tvp_LoadingConfirmationExistingFile"));

        await _dbExecutor.ExecuteAsync(sp, parameters);

        return true;
    }

    public async Task<bool> SubmitEvidenceAfter(MobileLoadingConfirmationEvidenceAfterQueueSubmit payload, string userId)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_EvidenceAfter_Submit";
        var fileTable = GenerateEvidenceFileTable(payload.Files);
        var existingFileTable = GenerateExistingEvidenceFileTable(payload.ExistingFiles);
        var parameters = new DynamicParameters();

        parameters.Add("@InstructionNo", payload.InstructionNo);
        parameters.Add("@ContainerNo", payload.ContainerNo);
        parameters.Add("@VehicleNo", payload.VehicleNo);
        parameters.Add("@SealNo", payload.SealNo);
        parameters.Add("@Remark", payload.Remark);
        parameters.Add("@DriverName", payload.DriverName);
        parameters.Add("@TransportVendor", payload.TransportVendor);
        parameters.Add("@Latitude", payload.Latitude);
        parameters.Add("@Longitude", payload.Longitude);
        parameters.Add("@RevisionReason", payload.RevisionReason);
        parameters.Add("@UserID", userId);
        parameters.Add("@Files", fileTable.AsTableValuedParameter("dbo.tvp_LoadingConfirmationEvidenceFile"));
        parameters.Add("@ExistingFiles", existingFileTable.AsTableValuedParameter("dbo.tvp_LoadingConfirmationExistingFile"));

        await _dbExecutor.ExecuteAsync(sp, parameters);

        return true;
    }

    public async Task<Dictionary<string, object>> CaptureEvidence(string instructionNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_Evidence_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new {InstructionNo = instructionNo});

        if (result == null) return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<LoadingConfirmationEvidenceBeforeDetailDto> GetEvidenceBeforeDetail(string instructionNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_EvidenceBefore_GetDetail";

        return await _dbExecutor.QueryMultipleAsync(sp, new
            {InstructionNo = instructionNo},
            async reader =>
            {
                var header = await reader.ReadFirstOrDefaultAsync<LoadingConfirmationEvidenceBeforeDetailDto>();

                if (header == null)
                {
                    return new LoadingConfirmationEvidenceBeforeDetailDto
                    {
                        HasData = false
                    };
                }

                var attachments = (await reader.ReadAsync<LoadingConfirmationEvidenceAttachmentDto>()).ToList();

                header.HasData = true;
                header.Attachments = attachments;

                return header;
            }
        );
    }

    public async Task<LoadingConfirmationEvidenceAfterDetailDto> GetEvidenceAfterDetail(string instructionNo)
    {
        string sp = "sp_Wms_Mobile_LoadingConfirmation_EvidenceAfter_GetDetail";

        return await _dbExecutor.QueryMultipleAsync(sp, new
        { InstructionNo = instructionNo },
            async reader =>
            {
                var header = await reader.ReadFirstOrDefaultAsync<LoadingConfirmationEvidenceAfterDetailDto>();

                if (header == null)
                {
                    return new LoadingConfirmationEvidenceAfterDetailDto
                    {
                        HasData = false
                    };
                }

                var attachments = (await reader.ReadAsync<LoadingConfirmationEvidenceAttachmentDto>()).ToList();

                header.HasData = true;
                header.Attachments = attachments;

                return header;
            }
        );
    }
}
