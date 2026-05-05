create procedure [dbo].[sp_Wms_Robot_SetTrolley_Capture]
	@RequestID varchar(50)
AS
BEGIN
	SELECT @RequestID RequestID, Trolley_No [TrolleyNo] FROM  PartMaterialRequestDetail 
	where RefNumber = @RequestID
END
