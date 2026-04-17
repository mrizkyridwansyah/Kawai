using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/export-file")]
[ApiController]
public class ExportFileController : HahaController
{
    [HttpGet("download-excel")]
    public IActionResult DownloadFileExportExcel(string key)
    {
        var file = FileStorage.GetFromExports(key);

        if (file == null)
            return Invalid("File Export tidak ditemukan");

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        ms.Position = 0;

        file.Dispose(); // dispose source stream

        FileStorage.RemoveFromExports(key);

        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "export.xlsx");
    }

    [HttpGet("download-pdf")]
    public IActionResult DownloadFileExportPDF(string key)
    {
        var file = FileStorage.GetFromExports(key);

        if (file == null)
            return Invalid("File Export tidak ditemukan");

        if (file.CanSeek)
            file.Position = 0;

        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(file, "application/pdf", "export");
    }
}
