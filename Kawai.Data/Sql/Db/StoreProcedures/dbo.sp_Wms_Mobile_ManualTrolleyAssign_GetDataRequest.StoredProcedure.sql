
CREATE   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_GetDataRequest]
	@RequestNo varchar(50)
as
begin
	if not exists (SELECT 1 fROM PartMaterialRequestDetail WHERE RefNumber = @RequestNo)
	begin
		raiserror('Data Request tidak ditemukan!', 16, 1)
		return
	end
	SELECT 
		dtl.RefNumber RequestNo, dtl.Trolley_No TrolleyNo, dtl.IsCurrentProcessManual, trl.Description TrolleyNoDesc,
		LastStopPointComplete = 
		isnull((
			select top 1 Stop_Point 
			from PartMaterialRequestSendRobotDetail 
			where RequestSendID = @RequestNo and [Status] = 1 
			order by Pickup_Seq desc
		), '')
	fROM PartMaterialRequestDetail dtl
	LEFT JOIN MS_Trolley trl on dtl.Trolley_No = trl.TrolleyCode
	WHERE RefNumber = @RequestNo
end
