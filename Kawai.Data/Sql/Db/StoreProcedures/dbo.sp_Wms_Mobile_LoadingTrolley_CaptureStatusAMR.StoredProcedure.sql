
create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_CaptureStatusAMR]
	@PickingNo varchar(100),
	@StopPoint varchar(25)
as
begin
	select
		StatusAMR, LastRequestDateAMR, LastUserRequestAMR 
	From PartMaterialRequestSendRobotDetail 
	where RequestSendID = @PickingNo and Stop_Point = @StopPoint
end
