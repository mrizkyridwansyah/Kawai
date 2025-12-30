SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER   procedure [dbo].[sp_Wms_Physical_Inventory_Capture]
	@WarehouseCode	nvarchar(50)
	,@ItemCode		nvarchar(50)
as
begin
	select	[Warehouse_Code]
			,[Item_Code]
			,[LM_PreMonth]
			,[LM_Receipt]
			,[LM_Supply]
			,[LM_LossReject]
			,[LM_Current]
			,[LM_Inventory]
			,[LM_Reason]
			,[TM_PreMonth]
			,[TM_Receipt]
			,[TM_Supply]
			,[TM_LossReject]
			,[TM_Current]
			,[TM_Inventory]
			,[TM_Reason]
			,[NM_PreMonth]
			,[NM_Receipt]
			,[NM_Supply]
			,[NM_LossReject]
			,[NM_Current]
			,[NM_Inventory]
			,[NM_Reason]
			,[Last_Update]
			,[Last_User]
	From Stock_Master 
	Where Warehouse_Code = @WarehouseCode
	and Item_Code = @ItemCode

end