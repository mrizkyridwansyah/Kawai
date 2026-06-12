

CREATE procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_UpdateStatusAMR]
	@RequestNo varchar(100),
	@TrolleyNo varchar(50),
	@LastStatus varchar(max)	
as
begin
	update PartMaterialRequestSendRobotDetail 
	set
		StatusAMR = @LastStatus
	where RequestSendID = @RequestNo and IsManual = 1 and isnull([Status], 0) = 1

	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	values (@RequestNo, @TrolleyNo, @TrolleyNo, 'Update Status AMR - API Cancel Request', 'sp_Wms_Mobile_ManualTrolleyAssign_UpdateStatusAMR', @LastStatus, GETDATE(), 'Robot')
end
