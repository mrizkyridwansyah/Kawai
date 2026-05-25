using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/loading-confirmation")]
[ApiController]
public class MobileLoadingConfirmationController : HahaController
{
    private readonly IMobileLoadingConfirmationRepository _repository;
    private readonly ITransactionProducer _transaction;
    private readonly DataLogger _logger;

    public MobileLoadingConfirmationController(IMobileLoadingConfirmationRepository repository, ITransactionProducer transaction, DataLogger logger)
    {
        _repository = repository;
        _transaction = transaction;
        _logger = logger;
    }

    [HttpGet("ddl-instruction")]
    public async Task<IActionResult> GetInstructionDDL(string keyword, string ids)
    {
        var results = await _repository.GetInstructionDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(x => x.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.InstructionNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list-shipping")]
    public async Task<IActionResult> GetListDetailShipping(string instructionNo, string keyword)
    {
        var result = await _repository.GetListDetailShipping(instructionNo, keyword);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo)
    {
        var result = await _repository.GetListDetail(instructionNo, barcodeNo, partNo, serialNo);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, string instructionNo)
    {
        var result = await _repository.GetDataBarcode(barcodeNo, instructionNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileLoadingConfirmationSubmit model)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader))
        {
            throw new Exception("Authorization header missing");
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
        if (Auth.User == null)
        {
            throw new Exception("Auth.User is NULL");
        }
        var payload = new
        {
            model.InstructionNo,
            model.BarcodeNo,
            DeviceID = remoteIp,
            Auth.User.UserID
        };

        var message = new StockTransactionMessage<MobileLoadingConfirmationSubmit>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "LOADING-CONFIRMATION-MOBILE",
            FormatMessage = "Loading Confirmation Mobile",
            Payload = model,
            LogContext = new LogContext
            {
                Method = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path,
                RemoteAddr = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                UserID = Auth.User.UserID,
                FullName = Auth.User.FullName
            }
        };

        try
        {
            _transaction.Publish<MobileLoadingConfirmationSubmit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpPost("submit-evidence-before")]
    public async Task<IActionResult> SubmitEvidenceBefore([FromForm] MobileLoadingConfirmationEvidenceBeforeSubmit model)
    {
        if (Auth.User == null)
            throw new Exception("Auth.User is NULL");

        bool hasNewFiles = model.Files != null && model.Files.Any(x => x.File != null);
        bool hasExistingFiles = model.ExistingFiles != null && model.ExistingFiles.Any();

        if (!hasNewFiles && !hasExistingFiles)
        {
            return BadRequest(new
            {
                Message = "Minimal 1 evidence file wajib ada."
            });
        }

        // =====================================================
        // PREPARE FILE QUEUE
        // =====================================================
        var files = new List<LoadingConfirmationEvidenceFileQueue>();

        // =====================================================
        // SAVE FILES TO SERVER
        // =====================================================
        foreach (var item in model.Files ?? new List<MobileLoadingConfirmationEvidenceFile>())
        {
            if (item.File == null)
                continue;

            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "loading-confirmation",
                "Evidence Before"
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{item.File.FileName}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await item.File.CopyToAsync(stream);
            }

            string relativePath = $"/uploads/loading-confirmation/Evidence Before/{fileName}";

            files.Add(new LoadingConfirmationEvidenceFileQueue
            {
                FileName = item.File.FileName,
                FilePath = relativePath,
                Remarks = item.Remarks
            });
        }

        // =====================================================
        // CREATE PAYLOAD FOR QUEUE
        // =====================================================
        var payload = new MobileLoadingConfirmationEvidenceBeforeQueueSubmit
        {
            InstructionNo = model.InstructionNo,
            ContainerNo = model.ContainerNo,
            VehicleNo = model.VehicleNo,
            SealNo = model.SealNo,
            Remark = model.Remark,
            DriverName = model.DriverName,
            TransportVendor = model.TransportVendor,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            RevisionReason = model.RevisionReason,
            Files = files,
            ExistingFiles = model.ExistingFiles
        };

        // =====================================================
        // CREATE MESSAGE
        // =====================================================
        var message = new StockTransactionMessage<MobileLoadingConfirmationEvidenceBeforeQueueSubmit>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "LOADING-CONFIRMATION-EVIDENCE-BEFORE",
            FormatMessage = "Loading Confirmation Evidence Before",
            Payload = payload,
            LogContext = new LogContext
            {
                Method = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path,
                RemoteAddr = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                UserID = Auth.User.UserID,
                FullName = Auth.User.FullName
            }
        };

        // =====================================================
        // PUBLISH QUEUE
        // =====================================================
        try
        {
            _transaction.Publish(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpPost("submit-evidence-after")]
    public async Task<IActionResult> SubmitEvidenceAfter([FromForm] MobileLoadingConfirmationEvidenceAfterSubmit model)
    {
        if (Auth.User == null)
            throw new Exception("Auth.User is NULL");

        bool hasNewFiles = model.Files != null && model.Files.Any(x => x.File != null);
        bool hasExistingFiles = model.ExistingFiles != null && model.ExistingFiles.Any();

        if (!hasNewFiles && !hasExistingFiles)
        {
            return BadRequest(new
            {
                Message = "Minimal 1 evidence file wajib ada."
            });
        }

        // =====================================================
        // PREPARE FILE QUEUE
        // =====================================================
        var files = new List<LoadingConfirmationEvidenceFileQueue>();

        // =====================================================
        // SAVE FILES TO SERVER
        // =====================================================
        foreach (var item in model.Files ?? new List<MobileLoadingConfirmationEvidenceFile>())
        {
            if (item.File == null)
                continue;

            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "loading-confirmation",
                "Evidence After"
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{item.File.FileName}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await item.File.CopyToAsync(stream);
            }

            string relativePath = $"/uploads/loading-confirmation/Evidence After/{fileName}";

            files.Add(new LoadingConfirmationEvidenceFileQueue
            {
                FileName = item.File.FileName,
                FilePath = relativePath,
                Remarks = item.Remarks
            });
        }

        // =====================================================
        // CREATE PAYLOAD FOR QUEUE
        // =====================================================
        var payload = new MobileLoadingConfirmationEvidenceAfterQueueSubmit
        {
            InstructionNo = model.InstructionNo,
            ContainerNo = model.ContainerNo,
            VehicleNo = model.VehicleNo,
            SealNo = model.SealNo,
            Remark = model.Remark,
            DriverName = model.DriverName,
            TransportVendor = model.TransportVendor,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            RevisionReason = model.RevisionReason,
            Files = files,
            ExistingFiles = model.ExistingFiles
        };

        // =====================================================
        // CREATE MESSAGE
        // =====================================================
        var message = new StockTransactionMessage<MobileLoadingConfirmationEvidenceAfterQueueSubmit>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "LOADING-CONFIRMATION-EVIDENCE-AFTER",
            FormatMessage = "Loading Confirmation Evidence After",
            Payload = payload,
            LogContext = new LogContext
            {
                Method = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path,
                RemoteAddr = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                UserID = Auth.User.UserID,
                FullName = Auth.User.FullName
            }
        };

        // =====================================================
        // PUBLISH QUEUE
        // =====================================================
        try
        {
            _transaction.Publish(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpGet("evidence-before-detail")]
    public async Task<IActionResult> GetEvidenceBeforeDetail(string instructionNo)
    {
        var result = await _repository.GetEvidenceBeforeDetail(instructionNo);
        return Success(result);
    }

    [HttpGet("evidence-after-detail")]
    public async Task<IActionResult> GetEvidenceAfterDetail(string instructionNo)
    {
        var result = await _repository.GetEvidenceAfterDetail(instructionNo);
        return Success(result);
    }
}