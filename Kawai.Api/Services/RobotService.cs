using DocumentFormat.OpenXml.EMMA;
using Hangfire;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models.Robot;
using System.Text.Json;

namespace Kawai.Api.Services;
public interface IRobotService
{
    Task CompletePicking(string requestNo);
    Task CompleteLoading(CompleteStatusRequest payload);
    Task CompleteLoadingSpecial(CompleteStatusRequest payload);
    Task CancelRequest(string requestNo, string trolleyNo);
}

public class RobotService : IRobotService
{
    private readonly HttpClient _client;
    private readonly DataLogger _changeDataLogger;
    private readonly ILogger<RobotService> _logger;
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyRepository;
    private readonly IMobileSupplyScanRequestRepository _supplyScanRequestRepository;
    private readonly IRobotRepository _robotRepository;

    public RobotService
    (
        IHttpClientFactory factory,
        IMobileLoadingTrolleyRepository loadingTrolleyRepository,
        IMobileSupplyScanRequestRepository supplyScanRequestRepository,
        IRobotRepository robotRepository,
        ILogger<RobotService> logger,
        DataLogger changeDataLogger
    )
    {
        _client = factory.CreateClient("robot");
        _loadingTrolleyRepository = loadingTrolleyRepository;
        _supplyScanRequestRepository = supplyScanRequestRepository;
        _robotRepository = robotRepository;
        _logger = logger;
        _changeDataLogger = changeDataLogger;
    }

    [AutomaticRetry(
        Attempts = 6,
        DelaysInSeconds = new int[] { 15, 15, 15, 15, 15, 15 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CompletePicking(string requestNo)
    {
        try
        {
            _logger.LogInformation(
                "Sending robot request for {RequestNoCode}",
                requestNo
            );

            var results = await _robotRepository.GetListData(requestNo);

            if (results == null || !results.Any()) return;

            string trolleyNo = results.First().TrolleyNo;
            if (!string.IsNullOrWhiteSpace(trolleyNo))
            {
                _logger.LogError($"Request No {requestNo} already has trolley number {trolleyNo} in robot request data.");
                return;
            }

            var payload = results
                .GroupBy(x => new
                {
                    x.RequestSendID,
                    x.LineCode,
                    x.WorkStationCode,
                    x.ProductionDate,
                    x.Model,
                    x.TrolleyCls,
                    x.PickingTime
                })
                .Select(g => new
                {
                    g.Key.RequestSendID,
                    g.Key.LineCode,
                    g.Key.WorkStationCode,
                    ProductionDate = g.Key.ProductionDate.ToString("yyyy-MM-dd"),
                    g.Key.Model,
                    g.Key.TrolleyCls,
                    PickupDatetime = g.Key.PickingTime,
                    Details = g
                        .OrderBy(x => x.PickupSequence)
                        .Select(x => new
                        {
                            x.RequestSendID,
                            x.StopPoint,
                            x.PickupSequence,
                            x.Status
                        })
                        .Distinct()
                        .ToList()
                })
                .ToList();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var response = await _client.PostAsJsonAsync(
                "/api/robot/send-request",
                payload[0],
                options
            );

            var result = await response.Content
                .ReadFromJsonAsync<RobotApiResponse>();

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!response.IsSuccessStatusCode)
                throw new Exception(message);

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            var before = await _supplyScanRequestRepository.CaptureStatusAMR(requestNo);

            await _supplyScanRequestRepository.UpdateStatusAMR(requestNo, message);

            var after = await _supplyScanRequestRepository.CaptureStatusAMR(requestNo);

            await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
            {
                DocumentType = "Robot - Send Request Complete Picking",
                EntityId = requestNo,
                ReferenceId = requestNo,
                Before = before,
                After = after,
                Action = DataLogAction.Update,
                Activity = "Send Request Complete Picking By Robot"
            }, "CompletePicking");

            _logger.LogInformation(
                "Robot API success for {RequestNoCode}. Message: {Message}",
                requestNo,
                message
            );
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            _logger.LogWarning(
                ex,
                "Timeout sending robot request for {RequestNoCode}",
                requestNo
            );

            await HandleFailureCompletePicking(
                requestNo,
                message
            );

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending robot request for {RequestNoCode}",
                requestNo
            );

            await HandleFailureCompletePicking(
                requestNo,
                ex.Message
            );

            throw;
        }
    }


    [AutomaticRetry(
        Attempts = 6,
        DelaysInSeconds = new int[] { 15, 15, 15, 15, 15, 15 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CompleteLoading(CompleteStatusRequest payload)
    {
        try
        {
            var request = await _robotRepository.GetRequestData(payload.RequestSendID, payload.StopPoint);

            if (request == null) return;

            if (request.IsComplete)
            {
                _logger.LogError($"Request No {payload.RequestSendID} status AMR already complete.");
                return;
            }

            // Return kalo status nya manual karena di sp updatestatusamr ada update current process manual jadi false biar asal update + biar ga banyak job nya
            if (request.IsManual)
            {
                _logger.LogError($"Request No {payload.RequestSendID} current status is manual.");
                return;
            }

            _logger.LogInformation("Sending robot request for {PickingNo}", payload.RequestSendID);

            var newPayload = new
            {
                payload.RequestSendID,
                payload.TrolleyNo,
                payload.StopPoint,
                payload.CompleteStatus
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var response = await _client.PostAsJsonAsync("/api/robot/complete-status", newPayload, options);

            var result = await response.Content
                .ReadFromJsonAsync<RobotApiResponse>();

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!response.IsSuccessStatusCode)
                throw new Exception(message);

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            var before = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            await _loadingTrolleyRepository.UpdateStatusAMR(payload.RequestSendID, payload.StopPoint, message);

            var after = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
            {
                DocumentType = "Robot - Send Request Complete Loading",
                EntityId = payload.RequestSendID + "|" + payload.StopPoint,
                ReferenceId = payload.RequestSendID + "|" + payload.StopPoint,
                Before = before,
                After = after,
                Action = DataLogAction.Update,
                Activity = "Send Request Complete Loading By Robot"
            }, "CompleteLoading");

            _logger.LogInformation(
                "Robot API success for {PickingNo}. Message: {Message}",
                payload.RequestSendID,
                message
            );
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            _logger.LogWarning(ex, "Timeout sending robot request for {PickingNo}", payload.RequestSendID);

            await HandleFailureCompleteLoading(
                payload.RequestSendID,
                payload.StopPoint,
                message
            );

            throw; // tetap retry
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for PickingNo {PickingNo}", payload.RequestSendID);

            await HandleFailureCompleteLoading(
                payload.RequestSendID,
                payload.StopPoint,
                ex.Message
            );

            throw;
        }
    }


    [AutomaticRetry(
        Attempts = 6,

        DelaysInSeconds = new int[] { 15, 15, 15, 15, 15, 15 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CompleteLoadingSpecial(CompleteStatusRequest payload)
    {
        try
        {
            _logger.LogInformation("Sending robot request for {PickingNo}", payload.RequestSendID);

            var request = await _robotRepository.GetRequestData(payload.RequestSendID, payload.StopPoint);

            if (request == null) return;

            if (request.IsComplete)
            {
                _logger.LogError($"Request No {payload.RequestSendID} status AMR already complete.");
                return;
            }

            // Return kalo status nya bukan manual biar ga banyak job nya
            if (!request.IsManual)
            {
                _logger.LogError($"Request No {payload.RequestSendID} current status already use AMR.");
                return;
            }

            var newPayload = new
            {
                payload.RequestSendID,
                payload.TrolleyNo,
                payload.StopPoint,
                payload.CompleteStatus
            };

            var response = await _client.PostAsJsonAsync("/api/robot/complete-status-special", newPayload);

            var result = await response.Content
                .ReadFromJsonAsync<RobotApiResponse>();

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!response.IsSuccessStatusCode)
                throw new Exception(message);

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            var before = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            await _loadingTrolleyRepository.UpdateStatusAMR(payload.RequestSendID, payload.StopPoint, message);

            var after = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
            {
                DocumentType = "Robot - Send Request Complete Loading",
                EntityId = payload.RequestSendID + "|" + payload.StopPoint,
                ReferenceId = payload.RequestSendID + "|" + payload.StopPoint,
                Before = before,
                After = after,
                Action = DataLogAction.Update,
                Activity = "Send Request Complete Loading By Robot"
            }, "CompleteLoadingSpecial");

            _logger.LogInformation(
                "Robot API success for {PickingNo}. Message: {Message}",
                payload.RequestSendID,
                message
            );
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            _logger.LogWarning(ex, "Timeout sending robot request for {PickingNo}", payload.RequestSendID);

            await HandleFailureCompleteLoading(
                payload.RequestSendID,
                payload.StopPoint,
                message
            );

            throw; // tetap retry
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for PickingNo {PickingNo}", payload.RequestSendID);

            await HandleFailureCompleteLoading(
                payload.RequestSendID,
                payload.StopPoint,
                ex.Message
            );

            throw;
        }
    }


    [AutomaticRetry(
        Attempts = 6,
        DelaysInSeconds = new int[] { 15, 15, 15, 15, 15, 15 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CancelRequest(string requestNo, string trolleyNo)
    {
        try
        {
            var payload = new
            {
                RequestSendID = requestNo,
                TrolleyNo = trolleyNo
            };

            _logger.LogInformation("Sending robot request for {PickingNo}", payload.RequestSendID);

            var response = await _client.PostAsJsonAsync("/api/robot/cancel-request", payload);

            var result = await response.Content
                .ReadFromJsonAsync<RobotApiResponse>();

            var message = result?.Message
                ?? $"Robot API returned {response.StatusCode}";

            if (!response.IsSuccessStatusCode)
                throw new Exception(message);

            if (!string.Equals(result?.Status, "success", StringComparison.OrdinalIgnoreCase))
                throw new Exception(message);

            //var before = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            //await _loadingTrolleyRepository.UpdateStatusAMR(payload.RequestSendID, message);

            //var after = await _loadingTrolleyRepository.CaptureStatusAMR(payload.RequestSendID, payload.StopPoint);

            //await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
            //{
            //    DocumentType = "Robot - Send Request Complete Loading",
            //    EntityId = payload.RequestSendID + "|" + payload.StopPoint,
            //    ReferenceId = payload.RequestSendID + "|" + payload.StopPoint,
            //    Before = before,
            //    After = after,
            //    Action = DataLogAction.Update,
            //    Activity = "Send Request Complete Loading By Robot"
            //}, "CancelRequest");

            _logger.LogInformation(
                "Robot API success for {PickingNo}. Message: {Message}",
                payload.RequestSendID,
                message
            );
        }
        catch (TaskCanceledException ex)
        {
            const string message = "Timeout sending robot request";

            _logger.LogWarning(ex, "Timeout sending robot request for {PickingNo}", requestNo);

            throw; // tetap retry
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for PickingNo {PickingNo}", requestNo);
            throw;
        }
    }


    private async Task HandleFailureCompletePicking(string requestNoCode, string message)
    {
        var before = await _supplyScanRequestRepository.CaptureStatusAMR(requestNoCode);

        await _supplyScanRequestRepository.UpdateStatusAMR(requestNoCode, message);

        var after = await _supplyScanRequestRepository.CaptureStatusAMR(requestNoCode);

        await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
        {
            DocumentType = "Robot - Send Request Complete Picking",
            EntityId = requestNoCode,
            ReferenceId = requestNoCode,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Send Request Complete Picking By Robot"
        }, "CompletePicking");

        _logger.LogError(
            "Robot request failed for {RequestNoCode}. Message: {Message}",
            requestNoCode,
            message
        );
    }

    private async Task HandleFailureCompleteLoading(string pickingNo, string stopPoint, string message)
    {
        var before = await _loadingTrolleyRepository.CaptureStatusAMR(pickingNo, stopPoint);

        await _loadingTrolleyRepository.UpdateStatusAMR(pickingNo, stopPoint, message);

        var after = await _loadingTrolleyRepository.CaptureStatusAMR(pickingNo, stopPoint);

        await _changeDataLogger.SaveDataLogByRobot(new DataLogDto
        {
            DocumentType = "Robot - Send Request Complete Loading",
            EntityId = pickingNo,
            ReferenceId = pickingNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Send Request Complete Loading By Robot"
        }, "CompleteLoading");

        _logger.LogError(
            "Robot request failed for {PickingNo}. Message: {Message}",
            pickingNo,
            message
        );
    }
}

public class RobotApiResponse
{
    public string Status { get; set; } = default!;
    public string Message { get; set; } = default!;
    public object? Data { get; set; }
}