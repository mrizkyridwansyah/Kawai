using DocumentFormat.OpenXml.Office2010.Excel;
using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Kawai.Api.Templates
{
    public class PrintBarcodesA4Model : PageModel
    {
        public StockDetailDto Data { get; set; }
        public PrintBarcodesA4Model(StockDetailDto data)
        {
            Data = data;
        }

        public void OnGet()
        {
        }
    }
}
