SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_PhysicalInventory_GetListStock]
	@AddressCode varchar(25),
	@UserId varchar(25)
as
begin
	declare @WarehouseCode varchar(25), @AreaCode varchar(25) 

	select 
		@WarehouseCode = WarehouseCode, @AreaCode = AreaCode 
	From MS_Address where AddressCode = @AddressCode

	SELECT 
		sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.Qty CurrentQty, so.InventoryQty,
		StatusSO = case when so.InventoryQty is null then 'NOT_YET_SO' when so.InventoryQty = sd.Qty then 'OK' else 'DIFFERENT' end
	FROM StockDetail sd
	LEFT JOIN Item_Master mi on sd.ItemCode = mi.Item_Code
	LEFT JOIN StockOpname so on sd.BarcodeNo = so.BarcodeNo
	WHERE WarehouseCode = @WarehouseCode 
	and AreaCode = @AreaCode 
	and AddressCode = @AddressCode 
	and Qty > 0	
end
GO
