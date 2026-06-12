

CREATE   procedure [dbo].[sp_Wms_Mobile_SupplyScanRequest_SendRequestAMR]
	@RequestNo varchar(100),
	@UserId varchar(25)
as
begin
	-- cukup update yg trolley nya kosong aja
	update PartMaterialRequestDetail 
	set
		StatusAMR = 'Requesting To AMR',
		LastRequestDateAMR = getdate(),
		LastUserRequestAMR = @UserId
	where RefNumber = @RequestNo and isnull(Trolley_No, '') = ''

	insert into AMRRequestHistory (RequestNo, FromData, ToData, [Action], SourceAction, StatusAMR, RegisterDate, RegisterUser)
	SELECT TOP 1 RefNumber, Trolley_No, Trolley_No, 'SEND REQUEST FROM WMS', 'sp_Wms_Mobile_SupplyScanRequest_SendRequestAMR', 'Requesting to AMR', GETDATE(), 'Robot' 
	fROM PartMaterialRequestDetail
	WHERE RefNumber = @RequestNo
end
