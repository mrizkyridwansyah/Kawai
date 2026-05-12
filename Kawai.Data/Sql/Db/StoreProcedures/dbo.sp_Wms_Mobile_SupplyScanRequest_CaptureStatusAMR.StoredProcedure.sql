
create   procedure [dbo].[sp_Wms_Mobile_SupplyScanRequest_CaptureStatusAMR]
	@RequestNo varchar(100)
as
begin
	select 
		top 1 StatusAMR, LastRequestDateAMR, LastUserRequestAMR 
	From PartMaterialRequestDetail where RefNumber = @RequestNo
end
