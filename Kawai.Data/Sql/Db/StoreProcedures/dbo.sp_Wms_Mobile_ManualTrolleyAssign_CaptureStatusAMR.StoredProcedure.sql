

CREATE   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_CaptureStatusAMR]
	@PickingNo varchar(100)
as
begin
	select 
		 Stop_Point StopPoint, StatusAMR, LastRequestDateAMR, LastUserRequestAMR 
	From PartMaterialRequestSendRobotDetail 
	where RequestSendID = @PickingNo and isnull(Status, 0) = 1
end
