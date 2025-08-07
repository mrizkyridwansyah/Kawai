using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/notification")]
[ApiController]
public class NotificationController : HahaController
{
    private readonly INotificationRepository _notificationReporitory;
    public NotificationController(INotificationRepository notificationReporitory)
    {
        _notificationReporitory = notificationReporitory;
    }

    [HttpGet("unread-notif-by-receiver")]
    public async Task<IActionResult> UnreadNotifByReceiver(string receiver)
    {
        var results = await _notificationReporitory.UnreadNotifByReceiver(receiver);
        return Success(results);
    }

    [HttpGet("list-by-receiver")]
    public async Task<IActionResult> ListByReceiver(string receiver)
    {
        var results = await _notificationReporitory.GetAllNotifByUser(receiver);
        return Success(results);
    }

}
