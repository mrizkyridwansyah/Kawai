using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileMovingTrolleyTransactionHandler : ITransactionHandler
{
    private readonly IMobileMovingTrolleyRepository _movingTrolleyRepo;
    private readonly IRobotRepository _robotRepository;
    private readonly DataLogger _logger;

    public string TransactionType => "MOVING-TROLLEY-MOBILE";

    public MobileMovingTrolleyTransactionHandler(IMobileMovingTrolleyRepository MovingTrolleyRepo, IRobotRepository robotRepository, DataLogger logger)
    {
        _movingTrolleyRepo = MovingTrolleyRepo;
        _robotRepository = robotRepository;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileMovingTrolley>(json);

        var before = await _movingTrolleyRepo.Capture(model.TrolleyNo);

        await _movingTrolleyRepo.Save(model, userId);

        var after = await _movingTrolleyRepo.Capture(model.TrolleyNo);

        var scanInfo = await _robotRepository.GetSupplyScanRequestInfo(model.RequestNo);

        /*
         * Kalo Flag AMR Cls di Manufacture_Line = 1 tapi Flag AMR Cls di WorkstationLineSetting = 0 maka saat loading trolley harus kirim ke AMR.
         */
        if(scanInfo != null && scanInfo.LineAMRCls == "1" && scanInfo.WSAMRCls == "0")
        {
            BackgroundJob.Enqueue<IRobotService>(service => service.SendRequestSubLine(model.RequestNo, model.TrolleyNo, model.StopPoint, userId));
        }

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Moving Trolley",
            EntityId = model.RequestNo,
            ReferenceId = model.RequestNo,
            Before = before,
            After = after,
            Activity = "Save Mobile Moving Trolley",
            Action = DataLogAction.Update
        }, logContext);
    }
}
