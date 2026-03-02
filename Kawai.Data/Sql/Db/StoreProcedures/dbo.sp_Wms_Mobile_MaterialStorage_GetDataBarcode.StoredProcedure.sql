SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_Mobile_MaterialStorage_GetDataBarcode]
	@BarcodeNo varchar(100),
	@OnlyTemporary bit = 0
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

	if @OnlyTemporary = 1 and not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and StatusReceipt = 'OK' and AreaCode = 'TMP' and AddressCode = 'TMP' )
	begin
		raiserror('Lokasi stock sudah tidak berada di area temporary!', 16,1)
		return
	end

	SELECT sd.RefNo, sd.WarehouseCode, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.SublotNo, sd.Qty
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
GO
