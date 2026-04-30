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

        Response.OnCompleted(() =>
        {
            file.Dispose();
            return Task.CompletedTask;
        });

        return File(file,
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
