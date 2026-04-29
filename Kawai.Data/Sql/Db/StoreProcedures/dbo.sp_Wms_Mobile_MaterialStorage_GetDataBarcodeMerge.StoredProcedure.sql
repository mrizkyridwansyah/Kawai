

CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_MaterialStorage_GetDataBarcodeMerge]
	@BarcodeNo varchar(100)
as
begin
	declare @statusReceipt varchar(20), @qty numeric(18,9), @warehouse varchar(25)
	select @statusReceipt = StatusReceipt, @qty = Qty, @warehouse = WarehouseCode from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0

	if isnull(@qty, 0) = 0
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if isnull(@statusReceipt, '') not in ('OK', 'NG')
	begin
		raiserror('Hanya Status Stock OK / NG yang bisa digabungkan!', 16,1)
		return
	end

	if isnull(@warehouse, '') in (select Subcon_WH_Code from Trade_Master where Trade_Cls = '3') 
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	SELECT sd.RefNo, sd.WarehouseCode, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.SublotNo, sd.Qty, sd.StatusReceipt
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
