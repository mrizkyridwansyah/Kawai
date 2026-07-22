using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Data;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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


    [HttpGet("detailqty")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _bomworkstationRepository.GetDataQty(id);
        return Success(result);
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

    public static BOMWSImport ReadBOMWSImport(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File kosong.");

        using var stream = new MemoryStream();
        file.CopyTo(stream);

        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);

        var result = new BOMWSImport();

        int lastRow = ws.LastRowUsed().RowNumber();

        if (lastRow > 10000)
            throw new Exception("Upload gagal: maksimal 10.000 baris.");

        #region HEADER

        result.Header = new BOMWSHeaderImport();

        result.Header.LineCode = ws.Cell("A3").GetString()?.Trim();
        result.Header.ParentItemCode = ws.Cell("B3").GetString()?.Trim();
        result.Header.WorkStationCode = ws.Cell("D3").GetString()?.Trim();
        result.Header.TrolleyCls = ws.Cell("E3").GetString()?.Trim();
       

        var headerErrors = new List<string>();
        string lincode = result.Header.LineCode.Trim();
        string parentitemcode = result.Header.ParentItemCode.Trim();
        string workstationcode = result.Header.WorkStationCode.Trim();
        string trolleycls = result.Header.TrolleyCls.Trim();
       
        if (string.IsNullOrWhiteSpace(result.Header.LineCode))
            headerErrors.Add("Line Code wajib diisi");

        if (string.IsNullOrWhiteSpace(result.Header.ParentItemCode))
            headerErrors.Add("Parent Item Code wajib diisi");

        if (string.IsNullOrWhiteSpace(result.Header.WorkStationCode))
            headerErrors.Add("Workstation Code wajib diisi");

        if (string.IsNullOrWhiteSpace(result.Header.TrolleyCls))
            headerErrors.Add("Trolley Cls wajib diisi");

       

        // Length Validation
        if (!string.IsNullOrWhiteSpace(lincode) && lincode.Length > 25)
            headerErrors.Add("linecode maksimal 15 karakter.");

        if (!string.IsNullOrWhiteSpace(parentitemcode) && parentitemcode.Length > 25)
            headerErrors.Add("parent item code maksimal 25 karakter.");

        if (!string.IsNullOrWhiteSpace(workstationcode) && workstationcode.Length > 15)
            headerErrors.Add("workstation code maksimal 15 karakter.");
        
        if (!string.IsNullOrWhiteSpace(trolleycls) && trolleycls.Length > 15)
            headerErrors.Add("trolley cls maksimal 15 karakter.");

        //if (!string.IsNullOrWhiteSpace(bcno) && bcno.Length > 50)
        //    headerErrors.Add("BCNo maksimal 50 karakter.");



        result.Header.Errors = string.Join(", ", headerErrors);



        #endregion

        #region DETAIL

        int startRow = 6;

        for (int row = startRow; row <= lastRow; row++)
        {
            var detail = new BOMWSDetailImport
            {
                RowNumber = row
            };

            string childitem = ws.Cell(row, 1).GetValue<string>()?.Trim();
            string qtyText = ws.Cell(row, 2).GetValue<string>()?.Trim();
          
            bool allEmpty =
                string.IsNullOrWhiteSpace(childitem) &&
                string.IsNullOrWhiteSpace(qtyText);

            if (allEmpty)
                continue;

             detail.ChildItemCode = childitem;



            // Required Validation
         
            if (string.IsNullOrWhiteSpace(childitem))
                detail.Errors += $"Row {row}, Child Item wajib diisi. ";

            // Length Validation
            if (!string.IsNullOrWhiteSpace(childitem) && childitem.Length > 50)
                detail.Errors += $"Row {row}, Child Item maksimal 25 karakter. ";

          
            // Qty Validation
            if (!decimal.TryParse(qtyText, out decimal qty1))
            {
                detail.Errors += $"Row {row}, Qty harus berupa angka. ";
            }
            else
            {
                detail.Qty = qty1;

                if (qty1 <= 0)
                    detail.Errors += $"Row {row}, Qty harus lebih besar dari 0. ";
            }

            // Custom Validation
            detail.IsValid();

            result.Details.Add(detail);
        }

        #endregion

        #region VALIDASI AKHIR

        if (!result.Details.Any())
            throw new Exception("Detail BOM WS tidak ditemukan.");

        var duplicates = result.Details
            .GroupBy(x => new
            {
                 
                ChildItemCode = x.ChildItemCode?.Trim().ToUpper()
            })
            .Where(g => g.Count() > 1);

        foreach (var duplicate in duplicates)
        {
            foreach (var item in duplicate)
            {
                item.Errors += $"Row {item.RowNumber},  Child Item '{item.ChildItemCode}' duplicate dalam file. ";
            }
        }



        #endregion

        return result;
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(ImportModel payload)
    {
        try
        {
            Stopwatch timer = new();
            timer.Start();

            // Read Excel
            var importData = ReadBOMWSImport(payload.File);

            #region VALIDASI EXCEL

            bool hasHeaderError =
                !string.IsNullOrWhiteSpace(importData.Header?.Errors);

            bool hasDetailError =
                importData.Details.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Errors));

            if (hasHeaderError || hasDetailError)
            {
                timer.Stop();

                return ImportInvalid(
                    "DATA IMPORT TIDAK VALID",
                    importData
                );
            }

            #endregion

            #region VALIDASI DATABASE

            var dtDetail = DataTableHelper.ToDataTable(importData.Details);

            var validateResult = await _bomworkstationRepository.ValidateImport(
                importData.Header,
                dtDetail,
                Auth.User.UserID);

            // update header
            if (validateResult.Header != null)
            {
                importData.Header.Errors =
                    validateResult.Header.Errors;
            }

            // update detail
            importData.Details = validateResult.Details;

            bool hasDbHeaderError =
                !string.IsNullOrWhiteSpace(importData.Header?.Errors);

            bool hasDbDetailError =
                importData.Details.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Errors));

            if (hasDbHeaderError || hasDbDetailError)
            {
                timer.Stop();

                return ImportInvalid(
                    "DATA IMPORT TIDAK VALID",
                    importData
                );
            }

            #endregion

            #region EXECUTE

            if (payload.Action == "EXECUTE")
            {
                await _bomworkstationRepository.Import(
                    importData.Header,
                    dtDetail,
                    Auth.User.UserID,
                    payload.FactoryCode);

                await _logger.SaveDataLog(new DataLogDto
                {
                    DocumentType = "BOMWS",
                    EntityId = importData.Header.LineCode,
                    ReferenceId = importData.Header.ParentItemCode,
                    Action = DataLogAction.Import,
                    Activity = "Import BOMWS",
                    Before = null,
                    After = Newtonsoft.Json.Linq.JObject
                        .FromObject(importData)
                        .ToObject<Dictionary<string, object>>()
                });
            }

            #endregion

            timer.Stop();

            return Success(importData);
        }
        catch (Exception ex)
        {
            return Invalid(ex.Message);
        }
    }

}
