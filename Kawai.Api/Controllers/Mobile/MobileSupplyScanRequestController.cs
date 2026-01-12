using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile
{
    [Route("api/mobile/supplyscanrequest")]
    [ApiController]
    public class MobileSupplyScanRequestController : HahaController
    {
        private readonly IMobileSupplyScanRequestRepository _supplyscanrequestRepository;
        private readonly ITransactionProducer _transactionProducer;
        private readonly DataLogger _logger;
        public MobileSupplyScanRequestController(IMobileSupplyScanRequestRepository supplyscanrequestRepository, ITransactionProducer transactionProducer, DataLogger logger)
        {
            _supplyscanrequestRepository = supplyscanrequestRepository;
            _transactionProducer = transactionProducer;
            _logger = logger;
        }

        [HttpGet("ddlrequestno")]
        public async Task<IActionResult> DDLSearch(string keyword, string linecode, string requestno)
        {
            var results = await _supplyscanrequestRepository.GetRequestNoDDL(keyword, linecode);
            if (!string.IsNullOrEmpty(requestno))
            {
                var idList = requestno.Split(',').Select(id => id.Trim()).ToList();
                results = results.Where(x => idList.Contains(x.RequestNo)).ToList();
            }

            return Success(results);
        }

        [HttpGet("data-barcode")]
        public async Task<IActionResult> GetDataBarcode(string barcodeNo)
        {
            var result = await _supplyscanrequestRepository.GetDataBarcode(barcodeNo);
            return Success(result);
        }
    }
}
