using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/trade")]
[ApiController]
public class TradeController : HahaController
{
    private readonly ITradeRepository _tradeRepository;
    private readonly DataLogger _logger;

    public TradeController(ITradeRepository tradeRepository, DataLogger logger)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _tradeRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _tradeRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Trade_Code)).ToList();
        }

        return Success(results);
    }

    

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _tradeRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Trade model)
    {
        await _tradeRepository.Create(model, Auth.User.UserID);

        var after = await _tradeRepository.Capture(model.Trade_Code);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Trade",
            EntityId = model.Trade_Code,
            ReferenceId = model.Trade_Name,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Trade model)
    {
        var before = await _tradeRepository.Capture(model.Trade_Code);
        await _tradeRepository.Update(model, Auth.User.UserID);
        var after = await _tradeRepository.Capture(model.Trade_Code);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Trade",
            EntityId = model.Trade_Code,
            ReferenceId = model.Trade_Code,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _tradeRepository.Capture(id);
        await _tradeRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Trade",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["NG Code", "Description", "Common", "Last Update", "Last User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        var results = await _tradeRepository.GetAll(parameter);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.Trade_Code); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Trade_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Trade_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Trade_Name); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Trade_Abbr); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Contact_Person); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Address1); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Address2); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.City); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Country); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Country_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Country_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Epte_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Epte_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Region_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Region_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Postal_Code); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Telephone); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Fax); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Closing_Day); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Pay_Day); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.InvoicePay_Days); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Affiliate_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Affiliate_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Insurance_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Insurance_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NPWP_No); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NPWP_Name); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NPWP_Address); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NPWP_City); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NPPKP_No); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Invoice_To); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PO_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PO_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Price_Condition); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Price_Condition_Descs); 
            colIdx++;
             ExcelHelper.SetCell(row, colIdx, result.POPayment_Day); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POPayment_Terms); 
            colIdx++;
               ExcelHelper.SetCell(row, colIdx, result.Transportation_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Transportation_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POCaseMark1); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POCaseMark2); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POCaseMark3);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POCaseMark4); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POCaseMark5); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking1); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking2); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking3);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking4); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking5); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.POMarking6); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Subcon_WH_Code); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Subcon_WH_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NG_Cls); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NG_Cls_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SAP_Code); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Type_BC); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Type_BC_Descs); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.No_Izin); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.CODE_KPPBC); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NoIzin_Date.HasValue ? result.NoIzin_Date.Value.ToString("dd MMM yyyy HH:mm") : ""); 
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NITKU); 
 
        }

        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }

  
}
