SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Mobile_AssignStorage_GetDataBarcode]
	@BarcodeNo varchar(100) 
as
begin
	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and StatusReceipt = 'OK')
	begin
		raiserror('Status Stock belum OK!', 16,1)
		return
	end

	declare @refNo varchar(50) = (select RefNo from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)

	SELECT 
		sd.RefNo, 
		sd.WarehouseCode, wh.WarehouseName,
		sd.AreaCode, ar.AreaName,
		sd.AddressCode, ad.AddressName,
		sd.BarcodeNo, 
		sd.ItemCode, mi.Item_Name ItemName, 
		sd.LotNo, sd.SublotNo, sd.Qty CurrentQty
	FROM StockDetail sd
	left join vw_WarehouseLine wh on sd.WarehouseCode = wh.WarehouseCode
	left join vw_Area ar on sd.AreaCode = ar.AreaCode
	left join vw_Address ad on sd.AddressCode = ad.AddressCode
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE sd.RefNo = @refNo and sd.Qty > 0
end
GO
