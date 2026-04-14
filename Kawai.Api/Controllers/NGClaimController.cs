using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/ngclaim")]
[ApiController]
public class NGClaimController : HahaController
{
    private readonly INGClaimRepository _ngclaimRepository;
    private readonly DataLogger _logger;

    public NGClaimController(INGClaimRepository ngclaimRepository, DataLogger logger)
    {
        _ngclaimRepository = ngclaimRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long claimid)
    {
        var result = await _ngclaimRepository.GetDataHeader(claimid);
        return Success(result);
    }

    //[HttpPost("list-detail")]
    //public async Task<IActionResult> GetListDetail(long claimid)
    //{
    //    var results = await _ngclaimRepository.GetListDetail(claimid);
    //    return Success(results);
    //}

    [HttpPost("list-po-detail")]
    public async Task<IActionResult> GetListPODetail([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetListPODetail(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-ngclaim-detail")]
    public async Task<IActionResult> GetListNGClaimDetail([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetListNGClaimDetail(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NGClaim model)
    {
        await _ngclaimRepository.Create(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] NGClaim model)
    {
        var before = await _ngclaimRepository.Capture(model.ClaimId.Value);

        await _ngclaimRepository.Update(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpPatch("approve")]
    public async Task<IActionResult> Approve([FromBody] NGClaim model)
    {
        var before = await _ngclaimRepository.Capture(model.ClaimId.Value);

        await _ngclaimRepository.Approve(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material Approve",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword,   string supplier, DateTime? periodFrom, DateTime? periodUntil, string status,   string ids)
    {
        var results = await _ngclaimRepository.DDLSearch(keyword,  supplier, periodFrom, periodUntil, status,  Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ClaimID.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpPost("report-surat-jalan")]
    public async Task<IActionResult> ExportExcel(string factory, long claimid, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _ngclaimRepository.GetListReport(factory, claimid);
        if (results == null || !results.Any()) return Invalid("No Data");

        var fullHtml = await renderer.RenderAsync(
            "Templates/SuratJalan.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "SuratJalan_" + results[0].CustPONo);
    }


    //[HttpPost("report-surat-jalan")]
    //public async Task<IActionResult> ExportExcel(string factory, long claimid)
    //{
    //    var results = await _ngclaimRepository.GetListReport(factory, claimid);
    //    if (results == null || !results.Any()) return NoContent();

    //    using var workbook = new XLWorkbook();
    //    var ws = workbook.Worksheets.Add("Surat Jalan");

    //    int row = 1;

    //    // ===============================
    //    // HEADER PERUSAHAAN
    //    // ===============================
    //    ws.Cell(row, 6).Value = results[0].TanggalSurat.ToString();
  


    //    row += 2;
    //    ws.Cell(row, 1).Value = results[0].CompanyName.ToString();
    //    ws.Range(row, 1, row, 5).Merge().Style.Font.SetBold().Font.FontSize = 14;

    //    ws.Cell(row, 6).Value = "Surat Jalan";
    //    ws.Cell(row, 6).Style.Font.Bold = true;
    //    ws.Cell(row, 6).Style.Font.FontSize = 16;

    //    row++;

    //    ws.Cell(row, 1).Value = results[0].CompanyAddress.ToString();
    //    ws.Range(row, 1, row, 6).Merge();
    //    row++;

    //    ws.Cell(row, 1).Value =   results[0].Phone.ToString();  
    //    ws.Range(row, 1, row, 6).Merge();
    //    row += 2;

    //    // ===============================
    //    // JUDUL
    //    // ===============================
       
    //    row += 2;

    //    // ===============================
    //    // INFORMASI
    //    // ===============================
    //    ws.Cell(row, 1).Value = "No";
    //    ws.Cell(row, 2).Value =   ":" + results[0].No.ToString();

    //    ws.Cell(row, 5).Value = "Delivery To";
    //    ws.Cell(row, 6).Value = ":" + results[0].Delivery.ToString();
    //    row++;

    //    ws.Cell(row, 1).Value = "Cust PO No";
    //    ws.Cell(row, 2).Value = ":" + results[0].CustPONo.ToString();
    //    row++;

    //    ws.Cell(row, 1).Value = "BC Type";
    //    ws.Cell(row, 2).Value = ":" + results[0].BCType.ToString();
    //    row++;

    //    ws.Cell(row, 1).Value = "BC Number";
    //    ws.Cell(row, 2).Value = ":" + results[0].BCNumber.ToString();

    //    ws.Cell(row, 5).Value = "Model";
    //    ws.Cell(row, 6).Value = ":" + results[0].Model.ToString();
    //    row++;

    //    ws.Cell(row, 1).Value = "QTY";
    //    ws.Cell(row, 2).Value = ":" + results[0].Qty.ToString(); 
    //    row += 2;

    //    // ===============================
    //    // KALIMAT PENGIRIMAN
    //    // ===============================
    //    ws.Cell(row, 1).Value = "Kami Kirimkan barang-barang tersebut dibawah ini dengan kendaraan:" + results[0].Kendaraan.ToString() + "No " + results[0].NoKendaraan.ToString(); 
    //    ws.Range(row, 1, row, 6).Merge();
    //    row += 2;

    //    // ===============================
    //    // HEADER TABLE
    //    // ===============================
    //    ws.Cell(row, 1).Value = "No";
    //    ws.Cell(row, 2).Value = "Nama Part";
    //    ws.Cell(row, 3).Value = "Kode Part";
    //    ws.Cell(row, 4).Value = "QTY Pengiriman";
    //    ws.Cell(row, 5).Value = "Satuan";
    //    ws.Cell(row, 6).Value = "Keterangan";

    //    ws.Range(row, 1, row, 6).Style.Font.Bold = true;
    //    ws.Range(row, 1, row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    //    int tableStart = row;
    //    row++;

    //    int no = 1;

    //    foreach (var item in results)
    //    {
    //        ws.Cell(row, 1).Value = no++;
    //        ws.Cell(row, 2).Value = item.ItemName;
    //        ws.Cell(row, 3).Value = item.ItemCode;
    //        ws.Cell(row, 4).Value = item.QtyNG;
    //        ws.Cell(row, 5).Value = item.UnitCls;
    //        ws.Cell(row, 6).Value = item.Remarks;

    //        row++;
    //    }

    //    // ===============================
    //    // BORDER TABLE
    //    // ===============================
    //    var tableRange = ws.Range(tableStart, 1, row - 1, 6);
    //    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    //    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

    //    row += 2;

    //    ws.Cell(row, 1).Value = "* Please return this original letter  to " + results[0].CompanyName.ToString();
    //    ws.Range(row, 1, row, 6).Merge();

    //    // ===============================
    //    // FOOTER
    //    // ===============================
    //    ws.Cell(row, 1).Value = "Delivered by";
    //    ws.Cell(row, 3).Value = "Approved by";
    //    ws.Cell(row, 5).Value = "Checked by";
    //    ws.Cell(row, 6).Value = "Received by";
    //    ws.Range(row, 1, row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


    //    row += 4;

    //    ws.Cell(row, 1).Value = results[0].DeliveryByPosition.ToString();
    //    ws.Cell(row, 3).Value = results[0].ApprovedByPosition.ToString();
    //    ws.Cell(row, 5).Value = results[0].CheckedByPosition.ToString();
    //    ws.Cell(row, 6).Value = results[0].ReceivedByPosition.ToString();
    //    ws.Range(row, 1, row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

    //    ws.Columns().AdjustToContents();

    //    using var ms = new MemoryStream();
    //    workbook.SaveAs(ms);
    //    var fileBytes = ms.ToArray();
    //    var base64File = Convert.ToBase64String(fileBytes);

    //    return Success(base64File);
    //}


    //[HttpPost("print-label")]
    //public async Task<IActionResult> PrintLabel(NGClaim payload)
    //{
    //    if (!payload.ClaimId.HasValue) return Invalid("Data NG Claim Invalid");

    //    var before = await _ngclaimRepository.Capture(payload ?? 0);

    //    await _ngclaimRepository.PrintLabel(payload.ClaimId ?? 0, Auth.User.UserID);

    //    var after = await _ngclaimRepository.Capture(payload.ClaimId ?? 0);
    //    await _logger.SaveDataLog(new DataLogDto
    //    {
    //        DocumentType = "NG Claim Material",
    //        EntityId = (payload.ClaimId ?? 0).ToString(),
    //        ReferenceId = payload.ClaimNo,
    //        Before = before,
    //        After = after,
    //        Activity = "Print Label NG Claim",
    //        Action = DataLogAction.Update
    //    });
    //    return Success(after);
    //}


}
