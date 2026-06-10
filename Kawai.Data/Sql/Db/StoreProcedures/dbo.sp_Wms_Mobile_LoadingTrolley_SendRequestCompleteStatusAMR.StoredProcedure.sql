
create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_SendRequestCompleteStatusAMR]
	@PickingNo varchar(100),
	@StopPoint varchar(50),
	@UserId varchar(25)	
as
begin
	IF EXISTS (SELECT 1 FROM PartMaterialRequestDetail WHERE RefNumber = @PickingNo AND IsCurrentProcessManual = 1)
	BEGIN
		RAISERROR('Saat ini proses supply request sedang tidak menggunakan AMR', 16, 1)
		RETURN
	END

	update PartMaterialRequestSendRobotDetail 
	set 
		StatusAMR = 'Requesting to AMR', 
		LastUserRequestAMR = @UserId, 
		LastRequestDateAMR = getdate()
	where RequestSendID = @PickingNo and [Status] = 1 
	and Stop_Point = @StopPoint

	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	values (@PickingNo, @StopPoint, @StopPoint, 'SEND COMPLETE STATUS FROM WMS', 'sp_Wms_Mobile_LoadingTrolley_SendRequestCompleteStatusAMR', 'Requesting to AMR', GETDATE(), @UserId)

end
