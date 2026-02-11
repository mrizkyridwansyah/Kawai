using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Domain.Shared;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Kawai.Api.Services;

public static class ExcelHelper
{
    public static byte[] GenerateQrCode(string text, int scale = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(scale);
    }

    public static IXLStyle ApplyCellStyle(IXLCell cell, bool bold = false, bool italic = false,
        int fontSize = 12, XLAlignmentHorizontalValues hAlign = XLAlignmentHorizontalValues.Center,
        XLAlignmentVerticalValues vAlign = XLAlignmentVerticalValues.Center,
        XLColor? background = null)
    {
        var style = cell.Style;
        style.Font.Bold = bold;
        style.Font.Italic = italic;
        style.Font.FontSize = fontSize;
        style.Alignment.Horizontal = hAlign;
        style.Alignment.Vertical = vAlign;

        if (background != null)
        {
            style.Fill.BackgroundColor = background;
        }

        return style;
    }

    public static IXLStyle ApplyHeaderStyle(IXLWorksheet ws, int startRow, int endRow, int startCol, int endCol,
        bool bold = false, bool italic = false,
        int fontSize = 12, XLAlignmentHorizontalValues hAlign = XLAlignmentHorizontalValues.Center,
        XLAlignmentVerticalValues vAlign = XLAlignmentVerticalValues.Center,
        XLColor? background = null)
    {
        var range = ws.Range(startRow, startCol, endRow, endCol);
        var style = range.Style;
        style.Font.Bold = bold;
        style.Font.Italic = italic;
        style.Font.FontSize = fontSize;
        style.Alignment.Horizontal = hAlign;
        style.Alignment.Vertical = vAlign;
        style.Font.FontColor = XLColor.White;
        style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        if (background != null)
        {
            style.Fill.BackgroundColor = background;
        }

        return style;
    }

    public static void SetHeader(IXLWorksheet ws, int row, List<string> headers)
    {
        var headerRow = ws.Row(row);
        for (int i = 0; i < headers.Count; i++)
        {
            var cell = headerRow.Cell(i + 1);
            cell.Value = headers[i];
        }

        ApplyHeaderStyle(ws, row, row, 1, headers.Count, bold: true, background: XLColor.RoyalBlue);
    }

    public static void SetCell(IXLRow row, int colIdx, object? value, Action<IXLCell>? styling = null)
    {
        var cell = row.Cell(colIdx);

        // Langsung assign value as-is biar tipe data tetap (angka tetap angka, DateTime tetap DateTime)
        switch (value)
        {
            case null:
                cell.Value = "";
                break;
            case string s:
                cell.Value = s;
                break;
            case int i:
                cell.Value = i;
                break;
            case long l:
                cell.Value = l;
                break;
            case double d:
                cell.Value = d;
                break;
            case float f:
                cell.Value = Convert.ToDouble(f);
                break;
            case decimal dec:
                cell.Value = Convert.ToDouble(dec); // ClosedXML tidak support decimal langsung
                if (dec % 1 == 0)
                    cell.Style.NumberFormat.NumberFormatId = 3; // #,##0
                else
                    cell.Style.NumberFormat.Format = "#,##0.############";
                break;
            case DateTime dt:
                cell.Value = dt;
                break;
            case bool b:
                cell.Value = b;
                break;
            case Enum e:
                cell.Value = e.ToString();
                break;
            default:
                cell.Value = value.ToString()!;
                break;
        }

        //cell.Value = (XLCellValue)(value ?? "");

        // Kalau memang pakai styling, baru panggil
        if (styling != null)
            styling(cell);
    }
    public static void SetCell(IXLRow row, int colIdx, object? value)
    {
        SetCell(row, colIdx, value, null);
        //row.Cell(colIdx).Value = (XLCellValue)(value ?? "");
    }

    //public static void SetCell(IXLRow row, int colIdx, object value, Action<IXLCell>? styling = null)
    //{
    //    var cell = row.Cell(colIdx);
    //    cell.Value = value?.ToString() ?? "";
    //    styling?.Invoke(cell);
    //}

    public static void MergeCells(IXLWorksheet ws, int firstRow, int lastRow, int firstCol, int lastCol)
    {
        ws.Range(firstRow, firstCol, lastRow, lastCol).Merge();
    }

    public static void SetBorders(IXLRange range)
    {
        range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
    }

    public static void AutofitColumns(IXLWorksheet ws, int fromCol, int toCol)
    {
        for (int col = fromCol; col <= toCol; col++)
        {
            ws.Column(col).AdjustToContents();
        }
    }

    // Insert QR code sebagai gambar ke worksheet ClosedXML
    public static void InsertQRCode(IXLWorksheet ws, int row, int col, string qrText, int size = 100)
    {
        // Generate QR code bitmap pakai QRCoder
        byte[] qrPngBytes = GenerateQrCode(qrText, 20);

        using Bitmap qrBitmap = new Bitmap(new MemoryStream(qrPngBytes));
        using Bitmap resizedBitmap = new Bitmap(qrBitmap, new Size(size, size));

        // Convert bitmap ke MemoryStream PNG
        using var ms = new MemoryStream();
        resizedBitmap.Save(ms, ImageFormat.Png);
        ms.Position = 0;

        // Set ukuran cell supaya pas gambar
        int targetPixel = size + 10; // padding sedikit

        // Row height: 1 pixel ~ 0.75 point
        ws.Row(row).Height = targetPixel * 0.75;

        // Column width di ClosedXML: sekitar pixel / 6
        ws.Column(col).Width = targetPixel / 6.0;

        // **Hitung offset agar gambar tepat di tengah cell**
        // Ukuran cell dalam pixel
        double cellWidthPixels = ws.Column(col).Width * 7;  // kira-kira pixel
        double cellHeightPixels = ws.Row(row).Height / 0.75; // balik ke pixel

        // Offset supaya gambar di tengah (cellWidth - imageWidth)/2
        double xOffset = (cellWidthPixels - size) / 2;
        double yOffset = (cellHeightPixels - size) / 2;

        // Tambahkan gambar ke worksheet dengan offset
        var picture = ws.AddPicture(ms)
            .MoveTo(ws.Cell(row, col), (int)xOffset, (int)yOffset)
            .WithSize(size, size);
    }

    public static void InsertImage(IXLWorksheet ws, int row, int col, string imagePath, int size = 100)
    {
        using var fs = File.OpenRead(imagePath);

        // Set ukuran cell supaya pas gambar
        int targetPixel = size + 10; // padding sedikit

        // Row height: 1 pixel ~ 0.75 point
        ws.Row(row).Height = targetPixel * 0.75;

        // Column width di ClosedXML: sekitar pixel / 6 (boleh pakai 7 juga tergantung ketepatan)
        ws.Column(col).Width = targetPixel / 6.0;

        // Ukuran cell dalam pixel (estimasi)
        double cellWidthPixels = ws.Column(col).Width * 7;
        double cellHeightPixels = ws.Row(row).Height / 0.75;

        // Offset supaya gambar di tengah
        double xOffset = (cellWidthPixels - size) / 2;
        double yOffset = (cellHeightPixels - size) / 2;

        // Tambahkan gambar ke worksheet dengan offset agar center
        ws.AddPicture(fs)
            .MoveTo(ws.Cell(row, col), (int)xOffset, (int)yOffset)
            .WithSize(size, size);
    }

    public static string FormatPeriod(string yyyymm)
    {
        var year = int.Parse(yyyymm.Substring(0, 4));
        var month = int.Parse(yyyymm.Substring(4, 2));

        return new DateTime(year, month, 1).ToString("MMM-yy");
    }

    #region METOD BACA EXCEL UNTUK IMPORT
    public static List<T> ReadAndValidate<T>(IFormFile file)
            where T : ImportBase, new()
    {
        var result = new List<T>();

        if (file == null || file.Length == 0)
            return result;

        using var stream = new MemoryStream();
        file.CopyTo(stream);

        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);

        var props = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var colMap = new Dictionary<int, PropertyInfo>();

        var firstRow = ws.FirstRowUsed().RowNumber();
        var lastRow = ws.LastRowUsed().RowNumber();
        var firstCol = ws.FirstColumnUsed().ColumnNumber();
        var lastCol = ws.LastColumnUsed().ColumnNumber();

        // HEADER
        var headerRow = ws.Row(firstRow);
        for (int col = firstCol; col <= lastCol; col++)
        {
            var header = headerRow.Cell(col).GetString().Trim();
            if (string.IsNullOrWhiteSpace(header))
                continue;

            var prop = props.FirstOrDefault(p =>
            {
                // ambil DisplayAttribute jika ada
                var display = p.GetCustomAttribute<DisplayAttribute>();
                string displayName = display?.Name ?? p.Name;

                return string.Equals(displayName, header, StringComparison.OrdinalIgnoreCase);
            });

            if (prop != null)
                colMap[col] = prop;
        }

        // DATA ROWS
        for (int row = firstRow + 2; row <= lastRow; row++)
        {
            var xlRow = ws.Row(row);
            var item = new T { RowNumber = row };
            bool allEmpty = true;

            foreach (var kv in colMap)
            {
                var cell = xlRow.Cell(kv.Key);
                var prop = kv.Value;

                string value = cell.GetValue<string>()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(value))
                    allEmpty = false;

                if (string.IsNullOrEmpty(value))
                    continue;

                if (!TryConvertCell(value, prop.PropertyType, out var converted))
                {
                    item.Errors += $"Row {row}, Kolom '{prop.Name}' format tidak valid";
                    continue;
                }

                prop.SetValue(item, converted);
            }

            if (allEmpty)
                continue;

            // 🔥 VALIDASI OTOMATIS
            item.IsValid();

            result.Add(item);
        }

        return result;
    }

    private static bool TryConvertCell(string value, Type type, out object? result)
    {
        result = null;

        if (type == typeof(string))
        {
            result = value;
            return true;
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            if (value.Equals("YA", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
                return true;
            }
            if (value.Equals("TIDAK", StringComparison.OrdinalIgnoreCase))
            {
                result = false;
                return true;
            }
            return false;
        }

        if (type == typeof(int) || type == typeof(int?))
            return int.TryParse(value, out var i) && (result = i) != null;

        if (type == typeof(decimal) || type == typeof(decimal?))
            return decimal.TryParse(value, out var d) && (result = d) != null;

        if (type == typeof(double) || type == typeof(double?))
            return double.TryParse(value, out var db) && (result = db) != null;

        if (type == typeof(DateTime) || type == typeof(DateTime?))
            return DateTime.TryParse(value, out var dt) && (result = dt) != null;

        try
        {
            result = Convert.ChangeType(value, type);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion

}
