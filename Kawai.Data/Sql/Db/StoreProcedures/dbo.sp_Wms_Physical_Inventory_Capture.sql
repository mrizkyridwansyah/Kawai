
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure [dbo].[sp_Wms_Physical_Inventory_Capture]
	@Details	dbo.tvp_StockDetail READONLY
as
begin

	SELECT 
		sd.RefNo, sd.WarehouseCode, sd.AreaCode, sd.AddressCode,
		sd.BarcodeNo, sd.ItemCode, sd.LotNo, sd.Qty CurrentQty, so.InventoryQty, 
		StatusScan = case when so.InventoryQty is null then 'NOTYET' when so.InventoryQty = sd.Qty then 'SCANNED' else 'DIFFERENT' end,
		so.LastUpdate, so.LastUser
	FROM StockDetail sd
	LEFT JOIN StockOpname so 
		on sd.RefNo				= so.RefNo
		and sd.WarehouseCode	= so.WarehouseCode
		and sd.AreaCode			= so.AreaCode
		and sd.AddressCode		= so.AddressCode
		and sd.BarcodeNo		= so.BarcodeNo
		and sd.LotNo			= so.LotNo
		and sd.ItemCode			= so.ItemCode
	INNER JOIN @Details d 
		on sd.RefNo				= d.RefNo
		and sd.WarehouseCode	= d.WarehouseCode
		and sd.AreaCode			= d.AreaCode
		and sd.AddressCode		= d.AddressCode
		and sd.BarcodeNo		= d.BarcodeNo
		and sd.LotNo			= d.LotNo
		and sd.ItemCode			= d.ItemCode

end