using DocumentFormat.OpenXml.EMMA;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Kawai.Api.Robot.Controllers
{

    [ApiController]
    [Route("api/robot")]
    public class RobotController : HahaController
    {
        private readonly IRobotRepository _robotRepository;
        private readonly DataLogger _logger;

        public RobotController(IRobotRepository robotRepository, DataLogger logger)
        {
            _robotRepository = robotRepository;
            _logger = logger;
        }

        //jika mau pake HMAC
        //[Authorize(AuthenticationSchemes = "HmacScheme")]
        //[HttpPost("set-trolley")]
        //public IActionResult SetTrolley([FromBody] SetTrolley request)
        //{
        //    return Ok(new
        //    {
        //        Message = "Trolley Set Successfully",
        //        request.RequestID,
        //        request.TrolleyNo
        //    });
        //}

        //sekarang pake auth basic aja
        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpPost("set-trolley")]
        public async Task<IActionResult> SetTrolley([FromBody] SetTrolleyRequest payload)
        {
            if (string.IsNullOrWhiteSpace(payload.RequestID) || string.IsNullOrWhiteSpace(payload.TrolleyNo))
            {
                return BadRequest("RequestID dan TrolleyNo wajib diisi");
            }

            await _robotRepository.SetTrolleyAsync(payload);
            var after = await _robotRepository.CaptureSetTrolley(payload);
            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Robot - Set Trolley",
                EntityId = payload.RequestID,
                ReferenceId = payload.TrolleyNo,
                Before = null,
                After = after,
                Action = DataLogAction.Update
            });

            return Success(after,"Set Trolley success");
            
        }

        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpPost("complete-status")]
        public async Task<IActionResult> CompleteStatus([FromBody] CompleteStatusRequest payload)
        {
            await _robotRepository.CompleteStatusAsync(payload);
            var after = await _robotRepository.CaptureCompleteStatus(payload);
            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Robot - Complete Status",
                EntityId = payload.RequestID,
                ReferenceId = payload.StopPoint,
                Before = null,
                After = after,
                Action = DataLogAction.Update
            });
            return Success(after, "complete status success");
            
        }


        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpPost("moving-stoppoint")]
        public async Task<IActionResult> MovingStoppoint([FromBody] MovingTrolleyRequest payload)
        {
            return Ok(new
            {
                Message = "Moving Trolley Success"
              
            });
        }

        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpPost("empty-trolley")]
        public async Task<IActionResult> EmptyTrolley([FromBody] EmptyTrolleyRequest payload)
        {
            await _robotRepository.EmptyTrolleyAsync(payload);
            var after = await _robotRepository.CaptureEmptyTrolley(payload);
            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Robot - Empty Trolley",
                EntityId = payload.TrolleyNo,
                ReferenceId = payload.TrolleyNo,
                Before = null,
                After = after,
                Action = DataLogAction.Update
            });
            return Success(after, "Empty Trolley (" + payload.TrolleyNo + ") Success");
        }

        [HttpGet("list-data")]
        public async Task<IActionResult> GetListDetail()
        {
            var json = await _robotRepository.GetListData();
            return Content(json, "application/json");
        }


    }
}
