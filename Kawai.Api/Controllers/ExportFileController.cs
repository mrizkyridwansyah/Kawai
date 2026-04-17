using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/export-file")]
[ApiController]
public class ExportFileController : HahaController
{
    [HttpGet("download")]
    public IActionResult DownloadFileExport(string key)
    {
        var file = FileStorage.GetFromExports(key);

        if (file == null)
            return NoContent();

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        ms.Position = 0;

        file.Dispose(); // dispose source stream

        FileStorage.RemoveFromExports(key);

        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "export.xlsx");
    }
}
