using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kawai.Api.Templates
{
    public class SuratJalanModel : PageModel
    {
        public SuratJalanModel(List<NGClaimReportDto> datas)
        {
        }

        public void OnGet()
        {
        }
    }
}
