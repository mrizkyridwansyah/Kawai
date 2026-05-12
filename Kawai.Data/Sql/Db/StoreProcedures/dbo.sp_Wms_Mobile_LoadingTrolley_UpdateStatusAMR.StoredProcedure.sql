
create   procedure [dbo].[sp_Wms_Mobile_LoadingTrolley_UpdateStatusAMR]
	@PickingNo varchar(100),
	@StopPoint varchar(25),
	@LastStatus varchar(max)
as
begin
	-- cukup update yg status amr nya belom complete aja
	update PartMaterialRequestSendRobotDetail 
	set
		StatusAMR = @LastStatus
	where RequestSendDetailID = @PickingNo and Stop_Point = @StopPoint and isnull([Status], 0) = 1 and isnull(StatusAMR, '') <> 'Completed'

	-- update current process manual jadi false
	update PartMaterialRequestDetail set IsCurrentProcessManual = 0 where RefNumber = @PickingNo and IsCurrentProcessManual = 1
end
