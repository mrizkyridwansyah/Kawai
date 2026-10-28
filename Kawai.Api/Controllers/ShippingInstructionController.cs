using ClosedXML.Excel;
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

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportStuffingExcel([FromBody] RequestParameter parameter)
    {
        var results = await _shippingInstructionRepository.GetListReportPicking(parameter);
        if (results == null || !results.Any()) return NoContent();

        var resultheader = results.FirstOrDefault();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Suffing Data");

        int rowIdx = 1;

        // =========================
        // REPORT HEADER
        // =========================

        // Baris 1: Company Name & Form Code
        ws.Cell(rowIdx, 2).Value = "PT KAWAI INDONESIA PLANT-3";
        ws.Cell(rowIdx, 2).Style.Font.Bold = true;
        ws.Cell(rowIdx, 2).Style.Font.FontSize = 11;
        ws.Cell(rowIdx, 17).Value = "f-wh-05-RO"; // Pojok kanan atas
        ws.Cell(rowIdx, 17).Style.Font.Bold = true;
        rowIdx++;

        // Baris 2: Judul Report
        ws.Cell(rowIdx, 2).Value = "STUFFING REPORT";
        ws.Range(rowIdx, 2, rowIdx, 11).Merge();
        ws.Cell(rowIdx, 2).Style.Font.Bold = true;
        ws.Cell(rowIdx, 2).Style.Font.FontSize = 14;
        ws.Cell(rowIdx, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        ws.Cell(rowIdx, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Row(rowIdx).Height = 25;

        // Halaman (1 of 1) di sebelah kanan
        ws.Cell(rowIdx, 17).Value = "1 of 1";
        rowIdx++;
        rowIdx++; // Spacer

        // Baris 4: Info Header (S/I NO, TGL, Dest)
        ws.Cell(rowIdx, 2).Value = "S/I NO";
        ws.Cell(rowIdx, 4).Value = resultheader.SI_No;
        ws.Cell(rowIdx, 4).Style.Font.Bold = true;

        ws.Cell(rowIdx, 9).Value = "TGL : " + resultheader.SI_Date;
       

        ws.Cell(rowIdx, 11).Value = "Dest : " + resultheader.Destination;
          rowIdx++;

        // Baris 5: NO POL, LOADING BY, CHECKED BY, PREPARED
        ws.Cell(rowIdx, 2).Value = "NO POL";
        ws.Cell(rowIdx, 4).Value = resultheader.Vehicle_No;

        ws.Cell(rowIdx, 10).Value = "LOADING BY";
        ws.Cell(rowIdx + 2, 10).Value = resultheader.LoadingBy;

        ws.Cell(rowIdx, 14).Value = "CHECKED BY";
        ws.Cell(rowIdx + 2, 14).Value = resultheader.CheckedBy;

        ws.Cell(rowIdx, 17).Value = "PREPARED"; // Contoh kolom tambahan jika ada
        ws.Cell(rowIdx + 2, 17).Value = resultheader.PreparedBy; // Contoh kolom tambahan jika ada


       
        // Border tabel tanda tangan
        ws.Range("J5:L8").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range("M5:O8").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range("P5:R8").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range("J5:R5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


        rowIdx++;

        // Baris 6: CONTAINER NO
        ws.Cell(rowIdx, 2).Value = "CONTAINER NO";
        ws.Cell(rowIdx, 4).Value = resultheader.Container_No;
        rowIdx++;

        // Baris 7: SEAL NO
        ws.Cell(rowIdx, 2).Value = "SEAL NO";
        ws.Cell(rowIdx, 4).Value = resultheader.Seal_No;
        rowIdx++;

        // Baris 8: DO NO
        ws.Cell(rowIdx, 2).Value = "DO NO";
        ws.Cell(rowIdx, 4).Value = resultheader.DO_No;
        rowIdx++;
        rowIdx++; // Spacer tambahan sebelum tabel detail

        // =========================
        // TABLE HEADER (Multi-Column Layout per 6 Kolom)
        // =========================
        // Format template memiliki 3 blok data berdampingan (Kolom 2-6, Kolom 8-12, Kolom 14-18)
        int tableHeaderRow = rowIdx;

        // Blok 1
        ws.Cell(tableHeaderRow, 2).Value = "NO";
        ws.Cell(tableHeaderRow, 3).Value = "MODEL";
        ws.Cell(tableHeaderRow, 4).Value = "V / X";
        ws.Cell(tableHeaderRow, 5).Value = "SERIAL NO";
        ws.Cell(tableHeaderRow, 6).Value = "PLTNO";

        // Blok 2
        ws.Cell(tableHeaderRow, 8).Value = "NO";
        ws.Cell(tableHeaderRow, 9).Value = "MODEL";
        ws.Cell(tableHeaderRow, 10).Value = "V / X";
        ws.Cell(tableHeaderRow, 11).Value = "SERIAL NO";
        ws.Cell(tableHeaderRow, 12).Value = "PLTNO";

        // Blok 3
        ws.Cell(tableHeaderRow, 14).Value = "NO";
        ws.Cell(tableHeaderRow, 15).Value = "MODEL";
        ws.Cell(tableHeaderRow, 16).Value = "V / X";
        ws.Cell(tableHeaderRow, 17).Value = "SERIAL NO";
        ws.Cell(tableHeaderRow, 18).Value = "PLTNO";

        // Styling Header Tabel Utama
        var headerRange = ws.Range(tableHeaderRow, 2, tableHeaderRow, 18);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // =========================
        // TABLE DATA (Looping Multi-Column / Grid Layout)
        // =========================
        int dataRowIdx = tableHeaderRow + 1;
        int itemNo = 1;

        // Contoh jika data ingin dibagi menjadi 3 kolom secara horizontal per baris Excel
        int maxRowsPerBlock = (int)Math.Ceiling(results.Count / 3.0);

        for (int i = 0; i < maxRowsPerBlock; i++)
        {
            var currentRow = dataRowIdx + i;
            ws.Row(currentRow).Height = 20;

            // Item Blok 1 (Kolom 2 - 6)
            if (i < results.Count)
            {
                var item1 = results[i];
                ws.Cell(currentRow, 2).Value = item1.NoUrut;
                ws.Cell(currentRow, 3).Value = item1.Model;
                ws.Cell(currentRow, 4).Value = item1.Check;
                ws.Cell(currentRow, 5).Value = item1.Serial_No;
                ws.Cell(currentRow, 6).Value = item1.PalletNo;
            }

            // Item Blok 2 (Kolom 8 - 12)
            int index2 = i + maxRowsPerBlock;
            if (index2 < results.Count)
            {
                var item2 = results[index2];
                ws.Cell(currentRow, 8).Value = item2.NoUrut;
                ws.Cell(currentRow, 9).Value = item2.Model;
                ws.Cell(currentRow, 10).Value = item2.Check;
                ws.Cell(currentRow, 11).Value = item2.Serial_No;
                ws.Cell(currentRow, 12).Value = item2.PalletNo;
            }

            // Item Blok 3 (Kolom 14 - 18)
            int index3 = i + (maxRowsPerBlock * 2);
            if (index3 < results.Count)
            {
                var item3 = results[index3];
                ws.Cell(currentRow, 14).Value = item3.NoUrut;
                ws.Cell(currentRow, 15).Value = item3.Model;
                ws.Cell(currentRow, 16).Value = item3.Check;
                ws.Cell(currentRow, 17).Value = item3.Serial_No;
                ws.Cell(currentRow, 18).Value = item3.PalletNo;
            }
        }

        var lastDataRow = dataRowIdx + maxRowsPerBlock - 1;

        // Border untuk semua blok data tabel utama
        ws.Range(dataRowIdx, 2, lastDataRow, 6).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(dataRowIdx, 2, lastDataRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range(dataRowIdx, 8, lastDataRow, 12).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(dataRowIdx, 8, lastDataRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range(dataRowIdx, 14, lastDataRow, 18).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(dataRowIdx, 14, lastDataRow, 18).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


        // =========================
        // FOOTER REPORT
        // =========================

        // Footer dimulai 2 baris setelah baris terakhir tabel detail
        int footerRow = lastDataRow + 2;

        // Baris Sub QTY
        ws.Cell(footerRow, 2).Value = "Sub QTY : " + resultheader.TotalQty;
        ws.Cell(footerRow, 5).Value = "Pcs";

        // Baris tanda centang
        int checkRow = footerRow + 1;

        ws.Cell(checkRow, 10).Value = "✓";
        ws.Range(checkRow, 10, checkRow, 11).Merge();
        ws.Range(checkRow, 10, checkRow, 11).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        ws.Range(checkRow, 10, checkRow, 11).Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        // =========================
        // ITEM CHECK
        // =========================

        int itemCheckRow = footerRow + 2;

        ws.Cell(itemCheckRow, 2).Value = "ITEM CHECK";
        ws.Range(itemCheckRow, 2, itemCheckRow, 4).Merge();

        ws.Cell(itemCheckRow + 1, 2).Value = "1. ATAP";
        ws.Range(itemCheckRow + 1, 2, itemCheckRow + 1, 4).Merge();

        ws.Cell(itemCheckRow + 2, 2).Value = "2. DINDING";
        ws.Range(itemCheckRow + 2, 2, itemCheckRow + 2, 4).Merge();

        ws.Cell(itemCheckRow + 3, 2).Value = "3. LANTAI UTAMA";
        ws.Range(itemCheckRow + 3, 2, itemCheckRow + 3, 4).Merge();

        ws.Cell(itemCheckRow + 4, 2).Value = "4. LT SUDUT DAN PINGGIR";
        ws.Range(itemCheckRow + 4, 2, itemCheckRow + 4, 4).Merge();

        ws.Cell(itemCheckRow + 5, 2).Value = "5. PINTU";
        ws.Range(itemCheckRow + 5, 2, itemCheckRow + 5, 4).Merge();

        ws.Cell(itemCheckRow + 6, 2).Value = "6. PASANG PLASTIK (KHUSUS DEST JPN)";
        ws.Range(itemCheckRow + 6, 2, itemCheckRow + 6, 9).Merge();

        // =========================
        // STANDARD
        // =========================

        ws.Cell(itemCheckRow, 5).Value = "STANDARD";
        ws.Range(itemCheckRow, 5, itemCheckRow, 9).Merge();

        ws.Cell(itemCheckRow + 1, 5).Value = "TIDAK BOCOR";
        ws.Range(itemCheckRow + 1, 5, itemCheckRow + 1, 9).Merge();

        ws.Cell(itemCheckRow + 2, 5).Value = "TIDAK LENGKET / BERSIH";
        ws.Range(itemCheckRow + 2, 5, itemCheckRow + 2, 9).Merge();

        ws.Cell(itemCheckRow + 3, 5).Value = "BERSIHKAN DENGAN SAPU";
        ws.Range(itemCheckRow + 3, 5, itemCheckRow + 3, 9).Merge();

        ws.Cell(itemCheckRow + 4, 5).Value = "BERSIHKAN DENGAN SAPU";
        ws.Range(itemCheckRow + 4, 5, itemCheckRow + 4, 9).Merge();

        ws.Cell(itemCheckRow + 5, 5).Value = "TIDAK BOCOR";
        ws.Range(itemCheckRow + 5, 5, itemCheckRow + 5, 9).Merge();

        // Border tabel Item Check
        ws.Range(
            itemCheckRow, 2,
            itemCheckRow + 6, 11
        ).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range(
            itemCheckRow, 2,
            itemCheckRow + 6, 11
        ).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // =========================
        // DESCRIPTION / ACTION
        // =========================

        int actionHeaderRow = checkRow;

        ws.Cell(actionHeaderRow, 14).Value = "NO";
        ws.Cell(actionHeaderRow, 15).Value = "DESCRIPTION";
        ws.Range(actionHeaderRow, 15, actionHeaderRow, 16).Merge();

        ws.Cell(actionHeaderRow, 17).Value = "ACTION";
        ws.Range(actionHeaderRow, 17, actionHeaderRow, 18).Merge();

        // Baris kosong Description dan Action
        for (int i = 1; i <= 5; i++)
        {
            int r = actionHeaderRow + i;

            ws.Range(r, 15, r, 16).Merge();
            ws.Range(r, 17, r, 18).Merge();
        }

        // Border tabel Description / Action
        ws.Range(
            actionHeaderRow, 14,
            actionHeaderRow + 5, 18
        ).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        ws.Range(
            actionHeaderRow, 14,
            actionHeaderRow + 5, 18
        ).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // =========================
        // FINAL TOUCH & EXPORT
        // =========================
        ws.Column("B").Width = 3.34;
        ws.Column("C").Width = 15;
        ws.Column("D").Width = 4.43;
        ws.Column("E").Width = 9.42;
        ws.Column("F").Width = 6.14;
        ws.Column("G").Width = 1.29;
        ws.Column("H").Width = 3.34;
        ws.Column("I").Width = 15;
        ws.Column("J").Width = 4.43;
        ws.Column("K").Width = 9.42;
        ws.Column("L").Width = 6.14;
        ws.Column("M").Width = 1.29;
        ws.Column("N").Width = 3.34;
        ws.Column("O").Width = 15;
        ws.Column("P").Width = 4.43;
        ws.Column("Q").Width = 9.42;
        ws.Column("R").Width = 6.14;
        ws.Column("S").Width = 1.29;
 

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
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
