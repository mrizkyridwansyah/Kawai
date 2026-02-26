using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
[Route("api/bomworkstation")]
[ApiController]
public class BOMWorkStationController : HahaController
{
    private readonly IBOMWorkStationRepository _bomworkstationRepository;
    private readonly DataLogger _logger;

    public BOMWorkStationController(IBOMWorkStationRepository bomworkstationRepository, DataLogger logger)
    {
        _bomworkstationRepository = bomworkstationRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _bomworkstationRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("copydata")]
    public async Task<IActionResult> CopyBomWorkStation([FromBody] CopyBomWorkstation model)
    {
        
            // contoh simpan ke DB
            await _bomworkstationRepository.CopyBomWorkStation(
                model.FromLine,
                model.ToLine,
                model.ItemCode,
                Auth.User.UserID


            );
         
        return Success();
    }


    [HttpGet("listdetail")]
    public async Task<IActionResult> ListDetail(
     string linecode,
     string parentitem_code,
     string workstationcode)
    {
        var header = await _bomworkstationRepository
            .GetBOMWorkStationHeader(linecode, parentitem_code, workstationcode);

        var details = await _bomworkstationRepository
            .GetBOMWorkStationDetail(linecode, parentitem_code, workstationcode);

        var result = new
        {
            Header = header != null
                ? new[] { header }    
                : Array.Empty<object>(),

            BomSetting = details
        };

        return Success(result);
    }


    [HttpGet("ddl-modelcls-search")]
    public async Task<IActionResult> DDLModelClsSearch(string keyword, string ids)
    {
        var results = await _bomworkstationRepository.GetModelClsDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ModelCls)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-itembymodelcls-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string modelCls ,string ids)
    {
        var results = await _bomworkstationRepository.GetItemByModelClsDDL(keyword, modelCls);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ItemCode)).ToList();
        }

        return Success(results);
    }


    [HttpPost("save")]
    public async Task<IActionResult> Save(BOMWorkStation model)
    {
        var header = model.Header.FirstOrDefault();
        var before = await _bomworkstationRepository.Capture(header.ParentItem_Code, header.WorkStationCode);
        await _bomworkstationRepository.SaveBOMWorkStation(model,Auth.User.UserID);
        var after = await _bomworkstationRepository.Capture(header.ParentItem_Code, header.WorkStationCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "BOMWorkStation",
            EntityId = header.ParentItem_Code,
            ReferenceId = header.WorkStationCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _bomworkstationRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["WS Code", "Description", "Register User", "Register Date",   "Last User", "Last Update"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.WorkStationCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.WorkStationName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.RegisterUser);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.RegisterDate.HasValue ? result.RegisterDate.Value.ToString("dd MMM yyyy HH:mm") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUser);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
           
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
