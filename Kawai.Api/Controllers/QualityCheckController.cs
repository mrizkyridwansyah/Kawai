using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/qualitycheck")]
[ApiController]
public class QualityCheckController : HahaController
{
    private readonly IQualityCheckRepository _qualitycheckRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public QualityCheckController(IQualityCheckRepository qualitycheckRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _qualitycheckRepository = qualitycheckRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _qualitycheckRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpGet("detail")]
    public async Task<IActionResult> Get(long id)
    {
        var result = await _qualitycheckRepository.GetData(id);

        if (!String.IsNullOrEmpty(result.AttachmentFileName))
        {
            Stream? image = FileStorage.GetFromAttachments(result.AttachmentFileName);
            byte[] imageByte = null;

            if (image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    imageByte = memoryStream.ToArray();
                }
                image.Dispose();
            }

            result.AttachmentFileBase64 = imageByte;
        }

        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromForm] QualityCheckResult model)
    {
        var before = await _qualitycheckRepository.Capture(model.InspectionId);

        if (model.Attachment != null)
        {
            model.AttachmentName = String.IsNullOrEmpty(model.AttachmentName) ? "IQC_" + Guid.NewGuid().UniqueId(30) : model.AttachmentName;
            FileStorage.SaveToAttachments(model.AttachmentName, model.Attachment);
        }
        else
        {
            if (!String.IsNullOrEmpty(model.AttachmentName))
                FileStorage.RemoveFromAttachments(model.AttachmentName);

            model.AttachmentName = "";
        }

        await _qualitycheckRepository.Save(model, Auth.User.UserID);

        var after = await _qualitycheckRepository.Capture(model.InspectionId);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Quality Check IQC",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpPatch("confirm")]
    public async Task<IActionResult> Confirm(QualityCheckConfirm model)
    {
        if (model.InspectionResult == "SA")
        {
            var before = await _qualitycheckRepository.Capture(model.InspectionId);

            await _qualitycheckRepository.ConfirmSA(model, Auth.User.UserID);

            var after = await _qualitycheckRepository.Capture(model.InspectionId);

            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Quality Check IQC",
                EntityId = model.InspectionId.ToString(),
                ReferenceId = model.InspectionId.ToString(),
                Action = DataLogAction.Update,
                Activity = "Confirm Quality Check",
                Before = before,
                After = after
            });

            return Success(after);
        }
        else
        {
            var message = new StockTransactionMessage<QualityCheckConfirm>
            {
                AuthUserId = Auth.User.UserID,
                TimeStamp = EpochDateTime.Now,
                TransactionType = "IQC-RESULT-CONFIRM",
                FormatMessage = "Confirm Quality Check",
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
                _transactionProducer.Publish<QualityCheckConfirm>(message);
                return Pending(message);
            }
            catch (Exception ex)
            {
                throw new Exception("RabbitMQ unavailable: " + ex.Message);
            }
        }
    }

    [HttpPatch("approval-sa")]
    public async Task<IActionResult> ApprovalSA(QualityCheckConfirmSA model)
    {
        var message = new StockTransactionMessage<QualityCheckConfirmSA>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "IQC-RESULT-APPROVAL-SA",
            FormatMessage = "Approval Quality Check SA",
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
            _transactionProducer.Publish<QualityCheckConfirmSA>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpPost("print/report-ng")]
    public async Task<IActionResult> PrintReportNG(long receiptId, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _qualitycheckRepository.PrintReportNG(receiptId);
        if (results == null || !results.Any()) return Invalid("No Data NG");

        var fullHtml = await renderer.RenderAsync(
            "Templates/QCReport.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "QC_Report_" + results[0].DNNumber);
    }

    [HttpPost("print/report-ng-by-job")]
    public async Task<IActionResult> PrintReportNGByJob(long receiptId)
    {
        var results = await _qualitycheckRepository.PrintReportNG(receiptId);
        if (results == null || !results.Any()) return Invalid("No Data NG");

        string key = Guid.NewGuid().ToString();
        BackgroundJob.Enqueue<ExportService>(service => service.ExportPdfIQCReportNG(results, Auth.User.UserID, key));

        return Pending(message: "Data Report NG sedang diproses");
    }
}
