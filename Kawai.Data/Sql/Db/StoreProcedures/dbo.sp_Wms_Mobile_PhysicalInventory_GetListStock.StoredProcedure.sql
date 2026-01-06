SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_PhysicalInventory_GetListStock]
	@AddressCode varchar(25),
	@UserId varchar(25) 
as
begin
	declare @WarehouseCode varchar(25), @AreaCode varchar(25), @AddressName varchar(100)

	select 
		@WarehouseCode = WarehouseCode, @AreaCode = AreaCode, @AddressName = AddressName
	From MS_Address where AddressCode = @AddressCode

	SELECT 
		AddressCode = @AddressCode, AddressName = @AddressName, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.Qty CurrentQty, so.InventoryQty,
		StatusSO = case when so.InventoryQty is null then 'NOT_YET_SO' when so.InventoryQty = sd.Qty then 'OK' else 'DIFFERENT' end
	FROM StockDetail sd
	LEFT JOIN Item_Master mi on sd.ItemCode = mi.Item_Code
	LEFT JOIN StockOpname so on sd.BarcodeNo = so.BarcodeNo
	WHERE sd.WarehouseCode = @WarehouseCode 
	and sd.AreaCode = @AreaCode 
	and sd.AddressCode = @AddressCode 
	and sd.Qty > 0	
end
GO
