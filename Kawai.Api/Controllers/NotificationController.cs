using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/notification")]
[ApiController]
public class NotificationController : HahaController
{
    private readonly INotificationRepository _notificationReporitory;
    private readonly NotificationService<NotifApprovalHub> _notificationService;

    public NotificationController(INotificationRepository notificationReporitory, NotificationService<NotifApprovalHub> notificationService)
    {
        _notificationReporitory = notificationReporitory;
        _notificationService = notificationService;
    }

    [HttpPost("tes-notif")]
    public async Task<IActionResult> Test(string receiver)
    {
        List<Notification> notifications = new List<Notification>();
        Notification notification = new Notification
        {
            Title = "Ini untuk " + receiver,
            Description = $"Test Notification.",
            NotifType = "ERROR",
            Priority = "LOW",
            Receiver = receiver,
            Sender = receiver
        };

        notifications.Add(notification);

        await _notificationService.BroadCastOnlyTo([receiver], "NewNotification", new
        {
            notifications.Count,
            Notifications = notifications
        });

        return Success();
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

    [HttpPatch("update-seen")]
    public async Task<IActionResult> UpdateSeenNotification(long id)
    {
        await _notificationReporitory.UpdateSeenNotification(id);
        return Success();
    }

}
