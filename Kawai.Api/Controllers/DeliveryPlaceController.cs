using ClosedXML.Excel;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Api.Shared.Extension;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/deliveryplace")]
[ApiController]
public class DeliveryPlaceController : HahaController
{
    private readonly IDeliveryPlaceRepository _deliveryplaceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly DataLogger _logger;
    private readonly NotificationService<NotifApprovalHub> _notificationService;

    public DeliveryPlaceController(IDeliveryPlaceRepository deliveryplaceRepository, INotificationRepository notificationRepository, DataLogger logger, NotificationService<NotifApprovalHub> notificationService)
    {
        _deliveryplaceRepository = deliveryplaceRepository;
        _notificationRepository = notificationRepository;
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _deliveryplaceRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string trade_code,string keyword, string ids)
    {
        var results = await _deliveryplaceRepository.GetDDL(trade_code,keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Location_Code)).ToList();
        }

        return Success(results);
    }

    

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string trade_code, string id)
    {
        var result = await _deliveryplaceRepository.GetData(trade_code, id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] DeliveryPlace model)
    {
        await _deliveryplaceRepository.Create(model, Auth.User.UserID);

        var after = await _deliveryplaceRepository.Capture(model.Trade_Code,model.Location_Code);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delivery Place",
            EntityId = model.Location_Code,
            ReferenceId = model.Location_Code,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });


        /*
         *  INI CONTOH KALO MAU PAKE NOTIF SETELAH API BIKIN SESUATU

            List<string> receivers = ["ossas"];
            Notification notification = new Notification
            {
                Title = "Master Warehouse Created",
                Description = $"Warehouse {model.WarehouseCode} - {model.WarehouseName} has been created.",
                NotifType = "INFO",
                Priority = "LOW",
                Sender = Auth.User.UserID
            };

            foreach (var reciver in receivers)
            {
                notification.Receiver = reciver;
                await _notificationRepository.SaveNotification(notification);
            }

            await _notificationService.BroadCastOnlyTo(receivers, "NewNotification", new { Count = 1 });         
         */

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] DeliveryPlace model)
    {
        var before = await _deliveryplaceRepository.Capture(model.Trade_Code, model.Location_Code);
        await _deliveryplaceRepository.Update(model, Auth.User.UserID);
        var after = await _deliveryplaceRepository.Capture(model.Trade_Code, model.Location_Code);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delivery Place",
            EntityId = model.Location_Code,
            ReferenceId = model.Location_Code,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string trade_code, string id)
    {
        var before = await _deliveryplaceRepository.Capture(trade_code,id);
        await _deliveryplaceRepository.Remove(trade_code,id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delivery Place",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    
}
