using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kawai.Api.Templates
{
    public class SuratJalanSIModel : PageModel
    {
        public SuratJalanSIModel(List<ShippingInstructionReportDto> datas)
        {
        }

        public void OnGet()
        {
        }
    }
}
