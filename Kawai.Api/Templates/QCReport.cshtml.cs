using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kawai.Api.Templates
{
    public class QCReportModel : PageModel
    {
        public QCReportModel(List<QualityCheckReportDto> datas)
        {
        }

        public void OnGet()
        {
        }
    }
}
