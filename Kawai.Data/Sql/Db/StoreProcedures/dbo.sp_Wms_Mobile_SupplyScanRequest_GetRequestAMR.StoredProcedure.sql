CREATE procedure sp_Wms_Mobile_SupplyScanRequest_GetRequestAMR
	@RequestNo varchar(100)
as
begin
	if not exists (select 1 from PartMaterialRequestDetail where RefNumber = @RequestNo)
	begin
		raiserror('Data Request tidak ditemukan',16,1)
		return
	end

	select 
		RefNumber RequestNo, Trolley_No TrolleyNo, StatusAMR LastStatus, LastUserRequestAMR LastRequestUser, LastRequestDateAMR LastRequestDate,
		ValidToSendRequest = case when Trolley_No is null and RequestStatusID = 5 then cast(1 as bit) else cast(0 as bit) end
	From PartMaterialRequestDetail where RefNumber = @RequestNo
end