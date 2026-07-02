using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/barcode-traceability")]
[ApiController]
public class MobileBarcodeTraceabilityController : HahaController
{
    private readonly IMobileBarcodeTraceabilityRepository _repository;
    private readonly ITransactionProducer _transactionProducer;
    public MobileBarcodeTraceabilityController(IMobileBarcodeTraceabilityRepository repository, ITransactionProducer transactionProducer)
    {
        _repository = repository;
        _transactionProducer = transactionProducer;
    }

   
    [HttpGet("getbarcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _repository.GetDataBarcode(barcodeNo);
        return Success(result);
    }



}
