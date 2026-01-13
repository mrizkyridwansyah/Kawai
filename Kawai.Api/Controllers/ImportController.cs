using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

namespace Kawai.Api.Controllers;

[Route("api/import")]
[ApiController]
public class ImportController : HahaController
{
    private readonly IImportRepository _importRepository;

    public ImportController(IImportRepository importRepository)
    {
        _importRepository = importRepository;
    }

    [HttpGet("template")]
    public async Task<IActionResult> DownloadTemplate(string name)
    {
        var templateType = ImportableExtension.Templates
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (templateType == null)
            return Invalid("Template Import tidak ditemukan");

        var properties = templateType.GetProperties()
            .Where(p => p.DeclaringType != typeof(ImportBase)) // skip RowNumber & Errors
            .ToList();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(name);

        int colIdx = 1;

        foreach (var prop in properties)
        {
            #region HEADER (ROW 1)
            var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
            var columnName = displayAttr?.Name ?? prop.Name;

            ws.Cell(1, colIdx).Value = columnName;
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region INFO (ROW TIPE DATA)
            Type propType = prop.PropertyType;
            bool isNullable = false;

            if (Nullable.GetUnderlyingType(propType) != null)
            {
                isNullable = true;
                propType = Nullable.GetUnderlyingType(propType)!; // ambil T dari Nullable<T>
            }

            string typeInfo = propType.Name;

            if (prop.PropertyType == typeof(string))
            {
                var maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttr != null)
                    typeInfo += $"({maxLengthAttr.Length})";
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                typeInfo += Environment.NewLine + "Format: (yyyy/MM/dd)";
            }
            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                typeInfo += Environment.NewLine + "Format: (YA/TIDAK)";
            }
            else if (prop.PropertyType == typeof(decimal)
                  || prop.PropertyType == typeof(double)
                  || prop.PropertyType == typeof(float))
            {
                typeInfo += Environment.NewLine + "Format: (0.00)";
            }

            //if (isNullable)
            //    typeInfo += " (Nullable)";
            
            var cellInfo = ws.Cell(2, colIdx);
            cellInfo.Value = typeInfo;
            cellInfo.Style.Font.Italic = true;
            cellInfo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cellInfo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region REQUIRED
            var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttr != null)
            {
                cellInfo.Style.Fill.BackgroundColor = XLColor.LightPink;
                cellInfo.GetComment()
                        .AddText(requiredAttr.ErrorMessage ?? "Kolom ini WAJIB diisi");
            }
            #endregion

            #region REFERENCE SHEET
            var refAttr = prop.GetCustomAttribute<ReferenceSheetAttribute>();
            if (refAttr != null)
            {
                var sp = refAttr.StoreProcedure;
                cellInfo.GetComment()
                        .AddText($"\nReferensi ada di sheet '{prop.Name}'");

                if (!workbook.Worksheets.Any(s => s.Name == prop.Name))
                {
                    var refSheet = workbook.Worksheets.Add(prop.Name);
                    var referenceData = await GetReferenceData(sp);

                    if (referenceData.Any())
                    {
                        // HEADER
                        var headers = referenceData.First().Keys.ToList();
                        for (int c = 0; c < headers.Count; c++)
                        {
                            refSheet.Cell(1, c + 1).Value = headers[c];
                            refSheet.Cell(1, c + 1).Style.Font.Bold = true;
                            refSheet.Cell(1, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        // DATA
                        int r = 2;
                        foreach (var row in referenceData)
                        {
                            int c = 1;
                            foreach (var value in row.Values)
                            {
                                refSheet.Cell(r, c++).Value =
                                    value == null
                                        ? XLCellValue.FromObject(string.Empty)
                                        : XLCellValue.FromObject(value);
                            }
                            r++;
                        }

                        refSheet.Columns().AdjustToContents();
                        refSheet.SheetView.FreezeRows(1);
                        refSheet.RangeUsed().SetAutoFilter();
                    }
                }
            }
            #endregion

            colIdx++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);

        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"{name}.xlsx"
        );
    }

    [HttpPost("histories")]
    public async Task<IActionResult> GetImportHistories([FromBody] RequestParameter parameter)
    {
        var results = await _importRepository.GetImportHistories(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("download-key")]
    public async Task<IActionResult> GetFile(string id)
    {
        var result = await _importRepository.GetDataImportHistory(id);
        Stream? file = FileStorage.GetFromImports(id);

        if(file == null || result == null)
            return NotFound("File tidak ditemukan");

        return File(file, result.ContentType, result.FileName);
    }

    private async Task<List<IDictionary<string, object>>> GetReferenceData(string sp)
    {
        return (await _importRepository.GetReferenceSheet(sp)).ToList();
    }
}
