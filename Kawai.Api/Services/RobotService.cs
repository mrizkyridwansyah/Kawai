using Hangfire;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Api.Services;
public interface IRobotService
{
    Task CompletePicking(MobilSupplyScanRequestSubmit model, string userId);
    Task CompleteLoading(MobileLoadingTrolleyComplete model, string userId);
}

public class RobotService : IRobotService
{
    private readonly HttpClient _client;
    private readonly ILogger<RobotService> _logger;
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyRepository;
    private readonly IMobileSupplyScanRequestRepository _supplyScanRequestRepository;

    public RobotService(IHttpClientFactory factory, IMobileLoadingTrolleyRepository loadingTrolleyRepository, IMobileSupplyScanRequestRepository supplyScanRequestRepository, ILogger<RobotService> logger)
    {
        _client = factory.CreateClient("robot");
        _loadingTrolleyRepository = loadingTrolleyRepository;
        _supplyScanRequestRepository = supplyScanRequestRepository;
        _logger = logger;
    }

    [AutomaticRetry(
        Attempts = 5,
        DelaysInSeconds = new int[] { 10, 30, 60, 120, 300 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CompletePicking(MobilSupplyScanRequestSubmit model, string userId)
    {
        try
        {
            _logger.LogInformation("Sending robot request for {RequestNoCode}", model.RequestNoCode);

            //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, userId);

            var response = await _client.PostAsJsonAsync("/complete", model);

            if (!response.IsSuccessStatusCode)
            {
                //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, "FAILED", userId);
                _logger.LogError("Failed send to robot for Request No {RequestNoCode}", model.RequestNoCode);
                throw new Exception($"Robot API returned {response.StatusCode}");
            }

            //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, "SUCCESS", userId);

            _logger.LogInformation("Robot API success for {RequestNoCode}", model.RequestNoCode);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("Timeout sending robot request for {RequestNoCode}", model.RequestNoCode);
            throw; // tetap retry
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for Request No {RequestNoCode}", model.RequestNoCode);
            throw;
        }
    }

    [AutomaticRetry(
        Attempts = 5,
        DelaysInSeconds = new int[] { 10, 30, 60, 120, 300 },
        OnAttemptsExceeded = AttemptsExceededAction.Fail
    )]
    public async Task CompleteLoading(MobileLoadingTrolleyComplete model, string userId)
    {
        try
        {
            _logger.LogInformation("Sending robot request for {PickingNo}", model.PickingNo);

            //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, userId);

            var response = await _client.PostAsJsonAsync("/complete", model);

            if (!response.IsSuccessStatusCode)
            {
                //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, "FAILED", userId);
                _logger.LogError("Failed send to robot for PickingNo {PickingNo}", model.PickingNo);
                throw new Exception($"Robot API returned {response.StatusCode}");
            }

            //await _loadingTrolleyRepository.UpdateStatusRequestRobot(model, "SUCCESS", userId);

            _logger.LogInformation("Robot API success for {PickingNo}", model.PickingNo);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("Timeout sending robot request for {PickingNo}", model.PickingNo);
            throw; // tetap retry
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for PickingNo {PickingNo}", model.PickingNo);
            throw;
        }
    }
}