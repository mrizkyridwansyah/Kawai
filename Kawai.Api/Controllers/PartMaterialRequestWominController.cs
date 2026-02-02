using Kawai.Api.Services;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/supply-request/womin")]
[ApiController]
public class PartMaterialRequestWominController : HahaController
{
    private readonly IPartMaterialRequestWominRepository _partMaterialRequestWominRepository;
    private readonly DataLogger _logger;

    public PartMaterialRequestWominController(IPartMaterialRequestWominRepository partMaterialRequestWominRepository, DataLogger logger)
    {
        _partMaterialRequestWominRepository = partMaterialRequestWominRepository;
        _logger = logger;
    }

    [HttpPost("list-header")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestWominRepository.GetListHeader(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> ListDetail([FromBody] List<PartMaterialRequestWominModel> models)
    {
        var results = await _partMaterialRequestWominRepository.GetListDetail(models);
        return Success(results);
    }

    [HttpPost("list-stock")]
    public async Task<IActionResult> ListStock([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestWominRepository.GetListStock(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] List<PartMaterialRequestWominModel> models)
    {
        var logs = new List<DataLogDto>();

        foreach (var item in models)
        {
            var before = await _partMaterialRequestWominRepository.Capture(item.ProductionId);
            logs.Add(new DataLogDto
            {
                DocumentType = "Part Material Request Womin",
                EntityId = item.ProductionId.ToString(),
                ReferenceId = item.ProductionId.ToString(),
                Before = before,
                After = null,
                Action = DataLogAction.Update,
                Activity = "Save Part Material Request Womin"
            });
        }

        await _partMaterialRequestWominRepository.Save(models, Auth.User.UserID);

        foreach (var log in logs)
        {
            var after = await _partMaterialRequestWominRepository.Capture(log.EntityId.ToInt32());
            log.After = after;

            await _logger.SaveDataLog(log);
        }

        return Success();
    }

    [HttpPost("print-barcodes")]
    public async Task<IActionResult> Save([FromBody] List<string> barcodes, [FromServices] RazorViewRenderer renderer)
    {
        var renderedLabels = new List<string>();

        foreach (var label in barcodes)
        {
            StockDetailDto x = new StockDetailDto();
            x.BarcodeNo = label;
            var html = await renderer.RenderAsync(
                "Templates/PrintBarcode.cshtml",
                x);

            renderedLabels.Add(html);
        }

        var fullHtml = BuildA4Html(renderedLabels);
        //return Content(fullHtml, "text/html");
        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);

        return File(pdfBytes, "application/pdf", "labels.pdf");
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long requestId, string requestNo)
    {
        var before = await _partMaterialRequestWominRepository.CaptureRequest(requestId);

        await _partMaterialRequestWominRepository.Remove(requestId, Auth.User.UserID);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Womin",
            EntityId = requestNo,
            ReferenceId = requestNo,
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Part Material Request Womin"
        });

        return Success();
    }


    protected string BuildA4Html(List<string> labelHtmls)
    {
        var sb = new StringBuilder();

        sb.Append("""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="utf-8" />
                        <style>
                        @page {
                          size: A4;
                          margin: 10mm;
                        }

                        body {
                          margin: 0;
                          font-family: Arial, sans-serif;
                        }

                        .page {
                          width: 190mm;
                          height: 277mm;
                          display: grid;
                          grid-template-columns: repeat(2, 1fr);
                          grid-template-rows: repeat(4, 1fr);
                          gap: 5mm;
                          page-break-after: always;
                        }

                        .label {
                          border: 1px solid #000;
                          padding: 5mm;
                          box-sizing: border-box;
                        }

                        .label-inner {
                          font-size: 14pt;
                        }

                        </style>
                    </head>
                    <body>
            """);

        foreach (var chunk in labelHtmls.Chunk(8))
        {
            sb.Append("<div class='page'>");

            foreach (var label in chunk)
            {
                sb.Append("<div class='label'>");
                sb.Append(label);
                sb.Append("</div>");
            }

            sb.Append("</div>");
        }

        sb.Append("""
                    </body>
                    </html>
        """);

        return sb.ToString();
    }
}
