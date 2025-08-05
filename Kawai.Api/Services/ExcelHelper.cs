using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

    public static void SetHeader(IXLWorksheet ws, int row, List<string> headers)
    {
        var headerRow = ws.Row(row);
        for (int i = 0; i < headers.Count; i++)
        {
            var cell = headerRow.Cell(i + 1);
            cell.Value = headers[i];
            ApplyCellStyle(cell, bold: true, background: XLColor.RoyalBlue);
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
    }

    public static void SetCell(IXLRow row, int colIdx, object value, Action<IXLCell>? styling = null)
    {
        var cell = row.Cell(colIdx);
        cell.Value = value?.ToString() ?? "";
        styling?.Invoke(cell);
    }

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
}
