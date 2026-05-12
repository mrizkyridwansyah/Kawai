
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
end
