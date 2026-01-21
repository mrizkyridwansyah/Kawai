using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Data.Repositories.Mobile;
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

        [HttpGet("list-material")]
        public async Task<IActionResult> GetListDetailMaterial(string requestno)
        {
            var result = await _supplyscanrequestRepository.GetListDetailMaterial(requestno);
            result = result.ToList();
            return Success(result);
        }

        [HttpGet("list-detail")]
        public async Task<IActionResult> GetListDetail(string warehouseCode , string requestNo , string itemCode)
        {
            var result = await _supplyscanrequestRepository.GetListDetail(warehouseCode, requestNo, itemCode);
            result = result.ToList();
            return Success(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(MobilSupplyScanRequest model)
        {
            var before = await _supplyscanrequestRepository.Capture(model.BarcodeNo);

            await _supplyscanrequestRepository.Save(model, Auth.User.UserID);

            var after = await _supplyscanrequestRepository.Capture(model.BarcodeNo);

            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Mobile Supply Request Scan",
                EntityId = model.BarcodeNo.ToString(),
                ReferenceId = model.BarcodeNo.ToString(),
                Before = before,
                After = after,
                Activity = "Save Mobile Supply Request Scan",
                Action = DataLogAction.Update
            });
            return Success(after);

 
        }

    }
}
