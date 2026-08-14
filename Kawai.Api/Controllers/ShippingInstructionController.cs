using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/shipping-instruction")]
[ApiController]
public class ShippingInstructionController : HahaController
{
    private readonly IShippingInstructionRepository _shippingInstructionRepository;
    private readonly DataLogger _logger;


    public ShippingInstructionController(IShippingInstructionRepository shippingInstructionRepository, DataLogger logger)
     {
        _shippingInstructionRepository = shippingInstructionRepository;
        _logger = logger;
     
    }

    [HttpGet("po-ddlsearch")]
    public async Task<IActionResult> PODDLSearch(string keyword, string custCode, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string ids)
    {
        var results = await _shippingInstructionRepository.GetPODDL(keyword, custCode, typeDate, periodFrom, periodUntil, showOptionAll, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PONumber)).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("si-ddlsearch")]
    public async Task<IActionResult> SIDDLSearch(string keyword, string supplier, DateTime? periodFrom, DateTime? periodUntil , string sourceMenu, string orderEntry, string ids)
    {
        var results = await _shippingInstructionRepository.GetSIDDL(keyword, supplier, periodFrom, periodUntil, sourceMenu, orderEntry, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ShippingInstructionNo.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpPost("list-picking")]
    public async Task<IActionResult> ListPicking([FromBody] RequestParameter parameter)
    {
        var results = await _shippingInstructionRepository.GetListPicking(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-shipping-by-po")]
    public async Task<IActionResult> GetDataShippingByPO(string poNumber)
    {
        var result = await _shippingInstructionRepository.GetDataShippingByPO(poNumber);

        var grouped = result
            .GroupBy(x => new { x.Supplier, x.ShippingInstructionNo, x.ShippingInstructionDate, x.PONumber, x.DeliveryDate, x.IsExistsEvidenceLoading })
            .Select(g => new
            {
                g.Key.Supplier,
                g.Key.ShippingInstructionNo,
                g.Key.ShippingInstructionDate,
                g.Key.PONumber,
                g.Key.DeliveryDate,
                g.Key.IsExistsEvidenceLoading,
                Details = g.Select(x => new
                {
                    x.PO_SeqNo,
                    x.Item_Code,
                    x.Item_Name,
                    x.Unit_Cls,
                    x.Unit_Desc,
                    x.Qty,
                    x.Qty_Stock,
                    x.Qty_Picking,
                    x.Serial_No,
                    x.SerialNo_From,
                    x.SerialNo_To,
                }).ToList()
            })
            .FirstOrDefault();

        return Success(grouped);
    }

    [HttpGet("data-shipping")]
    public async Task<IActionResult> GetDataShipping(string shippingNo)
    {
        var result = await _shippingInstructionRepository.GetDataHeaderNew(shippingNo);

        var grouped = result
            .GroupBy(x => new { x.Supplier, x.ShippingInstructionNo, x.ShippingInstructionDate, x.PONumber, x.DeliveryDate, x.IsExistsEvidenceLoading })
            .Select(g => new
            {
                g.Key.Supplier,
                g.Key.ShippingInstructionNo,
                g.Key.ShippingInstructionDate,
                g.Key.PONumber,
                g.Key.DeliveryDate,
                g.Key.IsExistsEvidenceLoading,
                Details = g.Select(x => new
                {
                    x.PO_SeqNo,
                    x.Item_Code,
                    x.Item_Name,
                    x.Unit_Cls,
                    x.Unit_Desc,
                    x.Qty,
                    x.Qty_Stock,
                    x.Qty_Picking,
                    x.Serial_No,
                    x.SerialNo_From,
                    x.SerialNo_To,
                }).ToList()
            })
            .FirstOrDefault();

        return Success(grouped);
    }

    [HttpPost("save-picking")]
    public async Task<IActionResult> Save(ShippingInstructionPicking model)
    {
        var header = model.Header.FirstOrDefault();
        var before = await _shippingInstructionRepository.Capture(header.ShippingInstructionNo);
        await _shippingInstructionRepository.SavePicking(model, Auth.User.UserID);
        var after = await _shippingInstructionRepository.Capture(header.ShippingInstructionNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction Picking",
            EntityId = header.ShippingInstructionNo,
            ReferenceId = header.PONumber,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ShippingInstruction model)
    {
        if (model.Details == null || !model.Details.Any())
            return Invalid("Detail Order didn't exists!");

        await _shippingInstructionRepository.Create(model, Auth.User.UserID);

        var after = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo.ToString());
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction",
            EntityId = model.ShippingInstructionNo.ToString(),
            ReferenceId = model.ShippingInstructionNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] ShippingInstruction model)
    {
        var before = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo.ToString());

        await _shippingInstructionRepository.Update(model, Auth.User.UserID);

        var after = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction",
            EntityId = model.ShippingInstructionNo.ToString(),
            ReferenceId = model.ShippingInstructionNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

 
    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string shippinginstructionno)
    {
        var before = await _shippingInstructionRepository.Capture(shippinginstructionno);

        await _shippingInstructionRepository.Remove(shippinginstructionno);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delete Shipping Instruction ",
            EntityId = shippinginstructionno.ToString(),
            ReferenceId = shippinginstructionno.ToString(),
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Shipping Instruction "
        });

        return Success();
    }

 
    [HttpPost("report-surat-jalan")]
    public async Task<IActionResult> ExportExcel(string sino, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _shippingInstructionRepository.GetListReport(sino);
        if (results == null || !results.Any()) return Invalid("No Data");

        var fullHtml = await renderer.RenderAsync(
            "Templates/SuratJalan_SI.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "SuratJalan_" + results[0].CustPONo);
    }

    [HttpGet("list-evidence-loading-before")]
    public async Task<IActionResult> ListEvidenceLoadingBefore(string shippingNo)
    {
        var results = await _shippingInstructionRepository.GetListEvidenceLoading(shippingNo, "BEFORE");
        return Success(results);
    }

    [HttpGet("list-evidence-loading-after")]
    public async Task<IActionResult> ListEvidenceLoadingAfter(string shippingNo)
    {
        var results = await _shippingInstructionRepository.GetListEvidenceLoading(shippingNo, "AFTER");
        return Success(results);
    }

    [HttpGet("download-evidence-loading-before")]
    public async Task<IActionResult> DownloadEvidenceLoadingBefore(string shippingNo)
    {
        var relativePaths = await _shippingInstructionRepository.GetListEvidenceLoading(shippingNo, "BEFORE");
        if (relativePaths == null || relativePaths.Count == 0)
        {
            return NotFound("No evidence files found.");
        }

        return DownloadFilesAsZip(relativePaths, $"Evidence_Before_{shippingNo}.zip");
    }

    [HttpGet("download-evidence-loading-after")]
    public async Task<IActionResult> DownloadEvidenceLoadingAfter(string shippingNo)
    {
        var relativePaths = await _shippingInstructionRepository.GetListEvidenceLoading(shippingNo, "AFTER");
        if (relativePaths == null || relativePaths.Count == 0)
        {
            return NotFound("No evidence files found.");
        }

        return DownloadFilesAsZip(relativePaths, $"Evidence_After_{shippingNo}.zip");
    }

    private IActionResult DownloadFilesAsZip(List<string> relativePaths, string zipFileName)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            using (var zip = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                int index = 1;
                foreach (var relPath in relativePaths)
                {
                    if (string.IsNullOrWhiteSpace(relPath)) continue;

                    var trimmedPath = relPath.TrimStart('/', '\\').Replace('/', System.IO.Path.DirectorySeparatorChar);
                    var fullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", trimmedPath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        var entryName = System.IO.Path.GetFileName(fullPath);
                        if (string.IsNullOrEmpty(entryName))
                        {
                            entryName = $"Evidence_{index}.jpg";
                        }

                        var entry = zip.CreateEntry(entryName, System.IO.Compression.CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(fullPath))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                        index++;
                    }
                }
            }

            memoryStream.Position = 0;
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(memoryStream.ToArray(), "application/zip", zipFileName);
        }
    }
}
