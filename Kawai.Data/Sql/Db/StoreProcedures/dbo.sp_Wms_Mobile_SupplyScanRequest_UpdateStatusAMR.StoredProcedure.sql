
create   procedure [dbo].[sp_Wms_Mobile_SupplyScanRequest_UpdateStatusAMR]
	@RequestNo varchar(100),
	@LastStatus varchar(max)
as
begin
	-- cukup update yg trolley nya kosong aja
	update PartMaterialRequestDetail 
	set
		StatusAMR = @LastStatus
	where RefNumber = @RequestNo and isnull(Trolley_No, '') = ''

	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	SELECT TOP 1 RefNumber, Trolley_No, Trolley_No, 'Update Status AMR - API Send Request', 'sp_Wms_Mobile_SupplyScanRequest_UpdateStatusAMR', @LastStatus, GETDATE(), 'Robot' fROM PartMaterialRequestDetail
	WHERE RefNumber = @RequestNo
end
