using Hangfire;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Api.Services;
public interface IRobotService
{
    Task SendRobotRequest(MobileLoadingTrolleyComplete model);
}

public class RobotService : IRobotService
{
    private readonly ILogger<RobotService> _logger;

    public RobotService(ILogger<RobotService> logger)
    {
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new int[] { 10, 30, 60, 120, 300 })]
    public async Task SendRobotRequest(MobileLoadingTrolleyComplete model)
    {
        try
        {
            // Contoh: call HTTP API robot
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync("http://robot-api.local/complete", model);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed send to robot for PickingNo {PickingNo}", model.PickingNo);
                throw new Exception($"Robot API returned {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending robot request for PickingNo {PickingNo}", model.PickingNo);
            // Optional: bisa implement retry di sini
        }
    }
}