
create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_Capture]
	@RequestNo varchar(100)
as
begin
	select top 1 Trolley_No [TrolleyNo] From PartMaterialRequestDetail where RefNumber = @RequestNo
end
