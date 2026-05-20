using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/reprint")]
[ApiController]
public class ReprintController : HahaController
{
    private readonly IReprintRepository _reprintRepository;
    private readonly DataLogger _logger;

    public ReprintController(IReprintRepository reprintRepository, DataLogger logger)
    {
        _reprintRepository = reprintRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _reprintRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("printupdate")]
    public async Task<IActionResult> Printupdate([FromBody] List<Dictionary<string, object>> rows)
    {
        if (!rows.Any()) return NoContent();

        foreach (var rowMap in rows)
        {
            string key = rowMap.TryGetValue("Key", out var keyVal) ? keyVal?.ToString() ?? "" : "";
            string value = rowMap.TryGetValue("Value", out var val) ? val?.ToString() ?? "" : "";

            if (string.IsNullOrWhiteSpace(key))
                continue;

            await _reprintRepository.UpdatePrintValue(key, value, Auth.User.UserID);

        }

        

        return Success();
    }

    [HttpPost("printpdf")]
    public async Task<IActionResult> LabelBarcode(
     [FromBody] List<SelectedPrintDto> selectedPrint,
     [FromServices] RazorViewRenderer renderer)
    {
        try
        {
            if (selectedPrint == null || !selectedPrint.Any())
                return BadRequest("No barcode selected");

            var barcodeNos = selectedPrint
                .Select(x => x.Key)
                .ToList();

            var results = await _reprintRepository
                .GetListBarcodeDetail(barcodeNos);

            if (results == null || !results.Any())
                return BadRequest("Data barcode not found");

            var renderedLabels = new List<string>();

            foreach (var item in results)
            {
                var model = new LabelBarcodeDetailDto
                {
                    BarcodeNo = item.BarcodeNo,
                    FromCompany = item.FromCompany,
                    ToCompany = item.ToCompany,
                    PONumber = item.PONumber,
                    ShippingLot = item.ShippingLot,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    Qty = item.Qty,
                    DeliveryDate = item.DeliveryDate,
                    DNNumber = item.DNNumber,
                    ShippingLabelNo = item.ShippingLabelNo,
                };

                var html = await renderer.RenderAsync(
                    "Templates/PrintBarcode.cshtml",
                    model);

                renderedLabels.Add(html);
            }
 

          
            var fullHtml = BuildA4Html(renderedLabels);
            var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(pdfBytes, "application/pdf", $"Barcode_{DateTime.Now:yyyyMMddHHmmss}.pdf");

 
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
                                    /* pastikan TIDAK ada kotak */
                          border: none;
                          outline: none;
                          box-shadow: none;
                          page-break-after: always;
                        }

                        
                

              .label {
                width: 321px;
                margin: 5px auto;
                background: #fff;
                position: relative;
              }

              table {
                width: 100%;
                border-collapse: collapse;
              }

              td {
                padding: 0;
                vertical-align: top;
              }

              /* ===== HEADER ===== */
              .header {
                background: #fff;
                color: #000;
                font-weight: bold;
                height: 25px;
              }

              .header-title {
                font-size: 10px;
                padding-left: 5px;
            	padding-top: 8px;

              }

              .header-page {
                width: 90px;
                text-align: center;
                font-size: 10px;
            	padding-top: 8px;
              }

              /* ===== SHIPPING LOT (OVERLAY) ===== */
              .shipping-lot {
                position: absolute;
                top: 0;
                right: 0;
                width: 50px;
                border-left: 1px solid #000;
                border-bottom: 1px solid #000;
            	 border-right: 1px solid #000;
                background: #fff;
              }

              .shipping-lot-header {
                background: #fff;
                color: #000;
                text-align: center;
                padding: 3px 0;
                font-size: 6px;
              }

              .shipping-lot-number {
                text-align: center;
                font-size: 18px;
                font-weight: bold;
                padding: 4px 0;
                margin: 2px;
              }

              /* ===== FROM / TO ===== */
              .fromto td {
                width: 50%;
                padding: 2px 3px;
                border-top: 1px solid #000;
                border-bottom: 1px solid #000;
              }

              .fromto td:first-child {
                border-right: 1px solid #000;
              }

              .small {
              margin-top: 2px;
                font-size: 6px;
                font-weight: bold;
              }

              .big {
                margin-top: 3px;
                font-size: 9px;
                font-weight: bold;
              }

              /* ===== CONTENT ===== */
              .content td {
                padding: 5px;

              }

              .detail td {
                font-size: 10px;
                padding: 3px;


              }

              .lbl {
                width: 70px;

                font-weight: bold;
              }

              .colon {
                width: 5px;
              }

              .qr {
                text-align: right;
              }

              .qr img {
                width: 95px;
                height: 95px;

              }

              /* ===== FOOTER ===== */
              .footer {
                background: #fff;
                border-top: 1px solid #000;
              }

              .footer td {
                width: 50%;
                padding: 3px 4px;
              }

              .footer td:first-child {
                border-right: 1px solid #000;
              }

              .footer-title {
              margin-top: 2px;
                font-size: 7px;
                font-weight: bold;
              }

              .footer-value {
                margin-top: 8px;
            	 font-size: 10px;
                font-weight: bold;
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
