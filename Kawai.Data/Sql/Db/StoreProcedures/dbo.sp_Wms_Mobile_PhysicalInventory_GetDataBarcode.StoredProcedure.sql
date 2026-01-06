SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_PhysicalInventory_GetDataBarcode]
	@AddressCode varchar(25),
	@BarcodeNo varchar(50)
as
begin
	SELECT 
		sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.Qty CurrentQty, so.InventoryQty 
	FROM StockDetail sd
	LEFT JOIN Item_Master mi on sd.ItemCode = mi.Item_Code
	LEFT JOIN StockOpname so on sd.BarcodeNo = so.BarcodeNo
	WHERE sd.AddressCode = @AddressCode
	and sd.BarcodeNo = @BarcodeNo and sd.Qty > 0	
end
GO
