
create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_SendRequestCompleteStatusAMR]
	@PickingNo varchar(100),
	@StopPoint varchar(50),
	@UserId varchar(25)	
as
begin
	update PartMaterialRequestSendRobotDetail 
	set 
		StatusAMR = 'Requesting to AMR', 
		LastUserRequestAMR = @UserId, 
		LastRequestDateAMR = getdate()
	where RequestSendID = @PickingNo and [Status] = 1 
	and Stop_Point = @StopPoint
end
