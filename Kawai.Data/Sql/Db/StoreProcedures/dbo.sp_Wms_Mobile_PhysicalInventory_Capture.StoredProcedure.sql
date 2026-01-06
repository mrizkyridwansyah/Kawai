SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_PhysicalInventory_Capture]
	@BarcodeNo varchar(50)
as
begin
	SELECT 
		sd.BarcodeNo, sd.ItemCode, sd.LotNo, sd.Qty CurrentQty, so.InventoryQty, so.RefNo, so.WarehouseCode, so.AreaCode, so.AddressCode,
		StatusSO = case when so.InventoryQty is null then 'NOT_YET_SO' when so.InventoryQty = sd.Qty then 'OK' else 'DIFFERENT' end
	FROM StockDetail sd
	LEFT JOIN StockOpname so on sd.BarcodeNo = so.BarcodeNo
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0	
end
GO
