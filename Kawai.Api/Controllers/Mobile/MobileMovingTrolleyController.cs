using Kawai.Api.Services;
using Kawai.Data;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/moving-trolley")]
[ApiController]
public class MobileMovingTrolleyController : HahaController
{
    private readonly IHttpClientFactory _factory;
    private readonly HttpClient _client;
    private readonly IMobileMovingTrolleyRepository _movingTrolleyRepository;
    private readonly IRobotRepository _robotRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly ITrolleyRepository _trolleyRepository;

    public MobileMovingTrolleyController(IMobileMovingTrolleyRepository MovingTrolleyRepository, IRobotRepository robotRepository, ITransactionProducer transactionProducer, HttpClient client, ITrolleyRepository trolleyRepository, IHttpClientFactory factory)
    {
        _factory = factory;
        _movingTrolleyRepository = MovingTrolleyRepository;
        _robotRepository = robotRepository;
        _transactionProducer = transactionProducer;
        _client = factory.CreateClient("robot-sync");
        _trolleyRepository = trolleyRepository;
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var results = await _movingTrolleyRepository.GetDataTrolley(trolleyNo);
        var grouped = results
        .GroupBy(x => new { x.RequestNo })
        .Select(g => new
        {
            g.Key.RequestNo,
            Details = g.Select(x => new
            {
                x.WarehouseCode,
                x.WarehouseName,
                x.AreaCode,
                x.AreaName,
                x.AddressCode,
                x.AddressName,
                x.ItemCode,
                x.ItemName,
                x.TotalQty
            }).ToList()
        })
        .FirstOrDefault();

        return Success(grouped);
    }

    [HttpGet("data-stop-point")]
    public async Task<IActionResult> GetDataStopPoint(string stopPoint)
    {
        var result = await _movingTrolleyRepository.GetDataStopPoint(stopPoint);
        return Success(result);
    }

    [HttpPost("testing-amr-moving")]
    public async Task<IActionResult> TestingAMRMoving(MobileMovingTrolley model)
    {
        await _movingTrolleyRepository.CheckValidation(model);

        var scanInfo = await _robotRepository.GetSupplyScanRequestInfo(model.RequestNo);

        if (scanInfo != null && scanInfo.LineAMRCls == "1" && scanInfo.WSAMRCls == "0")
        {
            await SendRequestSubLine(model.RequestNo, model.TrolleyNo, model.StopPoint, Auth.User.UserID);
        }

        return Success();
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit(MobileMovingTrolley model)
    {
        await _movingTrolleyRepository.CheckValidation(model);

        var scanInfo = await _robotRepository.GetSupplyScanRequestInfo(model.RequestNo);
        /*
         * Kalo Flag AMR Cls di Manufacture_Line = 1 tapi Flag AMR Cls di WorkstationLineSetting = 0 maka saat loading trolley harus kirim ke AMR.
         */
        if (scanInfo != null && scanInfo.LineAMRCls == "1" && scanInfo.WSAMRCls == "0")
        {
            await SendRequestSubLine(model.RequestNo, model.TrolleyNo, model.StopPoint, Auth.User.UserID);
        }

        var message = new StockTransactionMessage<MobileMovingTrolley>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MOVING-TROLLEY-MOBILE",
            FormatMessage = "Moving Trolley Mobile",
            Payload = model,
            LogContext = new LogContext
            {
                Method = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path,
                RemoteAddr = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                UserID = Auth.User.UserID,
                FullName = Auth.User.FullName
            }
        };

        try
        {
            _transactionProducer.Publish<MobileMovingTrolley>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    private async Task UnbindRack(string requestNo, string trolleyNo)
    {
        try
        {
            //_logger.LogInformation(
            //    "Sending robot request for Unbind {TrolleyNo}",
            //    trolleyNo
            //);

            var payload = new
            {
                TrolleyNo = trolleyNo
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var response = await _client.PostAsJsonAsync(
                "/api/robot/unbind-rack-v2",
                payload,
                options
            );

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Robot API returned {(int)response.StatusCode} ({response.StatusCode}). Response: {body}"
                );
            }

            var result = await response.Content
                .ReadFromJsonAsync<RobotApiResponse>();

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            await _movingTrolleyRepository.UpdateStatusUnbindRackAMR(requestNo, trolleyNo, message);
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            //_logger.LogWarning(
            //    ex,
            //    "Timeout sending robot request for {RequestNoCode}",
            //    trolleyNo
            //);

            await _movingTrolleyRepository.UpdateStatusUnbindRackAMR(requestNo, trolleyNo, message);

            throw;
        }
        catch (Exception ex)
        {
            //_logger.LogError(
            //    ex,
            //    "Error sending robot request for {RequestNoCode}",
            //    trolleyNo
            //);

            await _movingTrolleyRepository.UpdateStatusUnbindRackAMR(requestNo, trolleyNo, ex.Message);

            throw;
        }
    }

    private async Task SendRequestSubLine(string requestNo, string trolleyNo, string stopPoint, string userId)
    {
        await SendRequestSubLineInternal(requestNo, trolleyNo, stopPoint, userId, false);
    }

    private async Task SendRequestSubLineInternal(string requestNo, string trolleyNo, string stopPoint, string userId, bool isRetry)
    {
        try
        {
            await _movingTrolleyRepository.SendRequestUnbindRackAMR(requestNo, trolleyNo, userId);
            await UnbindRack(requestNo, trolleyNo);
        }
        catch (Exception ex)
        {
            //_logger.LogWarning(ex, "UnbindRack failed for {TrolleyNo} in SendRequestSubLine, but continuing.", trolleyNo);
        }

        try
        {
            //_logger.LogInformation(
            //    "Sending robot request for SubLine {TrolleyNo}",
            //    trolleyNo
            //);

            var dataRequest = await _robotRepository.GetDataToSendRequestSubLine(requestNo, trolleyNo, stopPoint);

            if (dataRequest == null) return;

            var payloadSubLine = new
            {
                dataRequest.RequestSendID,
                dataRequest.LineCode,
                dataRequest.WorkStationCode,
                ProductionDate = dataRequest.ProductionDate.ToString("yyyy-MM-dd"),
                dataRequest.Model,
                dataRequest.TrolleyNo,
                dataRequest.PickupDatetime,
                dataRequest.StopPoint
            };

            await _movingTrolleyRepository.SendRequestSubLineAMR(requestNo, trolleyNo, stopPoint, userId);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var response = await _client.PostAsJsonAsync(
                "/api/robot/send-request-subline-v2",
                payloadSubLine,
                options
            );

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RobotApiResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                string? extractedTrolleyNo = null;
                try
                {
                    using (var doc = JsonDocument.Parse(body))
                    {
                        if (doc.RootElement.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Object)
                        {
                            if (dataProp.TryGetProperty("TrolleyNo", out var trolleyProp))
                            {
                                extractedTrolleyNo = trolleyProp.GetString();
                            }
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    //_logger.LogWarning(parseEx, "Failed to parse error response body for TrolleyNo.");
                }

                if (!isRetry && !string.IsNullOrEmpty(extractedTrolleyNo))
                {
                    var extractedTrolley = await _trolleyRepository.GetData(extractedTrolleyNo);

                    if (extractedTrolley != null && extractedTrolley.Trolley_Cls == dataRequest.TrolleyCls)
                    {
                        //_logger.LogWarning("SendRequestSubLine failed. Unbinding rack for TrolleyNo: {ExtractedTrolleyNo} and retrying.", extractedTrolleyNo);
                        await UnbindRack(requestNo, extractedTrolleyNo);
                        await SendRequestSubLineInternal(requestNo, trolleyNo, stopPoint, userId, true);
                        return;
                    }
                    else
                    {
                        string errorMessage = $"Trolley type mismatch. Extracted trolley type: '{extractedTrolley?.Trolley_Cls ?? "UNKNOWN"}' ({extractedTrolleyNo}), but parameter trolley type: '{dataRequest.TrolleyCls ?? "UNKNOWN"}' ({trolleyNo}).";
                        await _movingTrolleyRepository.UpdateStatusAMRSendRequestSubLine(requestNo, trolleyNo, stopPoint, errorMessage);
                        throw new HttpCustomException(400, $"Lokasi sudah ada Troli dengan Tipe BERBEDA! Silahkan Move ke area lain!");
                    }
                }

                throw new Exception(
                    $"Robot API returned {(int)response.StatusCode} ({response.StatusCode}). Response: {body}"
                );
            }

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            await _movingTrolleyRepository.UpdateStatusAMRSendRequestSubLine(requestNo, trolleyNo, stopPoint, message);
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            //_logger.LogWarning(
            //    ex,
            //    "Timeout sending robot request for {RequestNoCode}",
            //    trolleyNo
            //);

            await _movingTrolleyRepository.UpdateStatusAMRSendRequestSubLine(requestNo, trolleyNo, stopPoint, message);

            throw;
        }
        catch (Exception ex)
        {
            //_logger.LogError(
            //    ex,
            //    "Error sending robot request for {RequestNoCode}",
            //    trolleyNo
            //);

            await _movingTrolleyRepository.UpdateStatusAMRSendRequestSubLine(requestNo, trolleyNo, stopPoint, ex.Message);

            throw;
        }
    }

}
