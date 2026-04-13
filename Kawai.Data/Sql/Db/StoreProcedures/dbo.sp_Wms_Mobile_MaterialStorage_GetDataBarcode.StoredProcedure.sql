SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_Mobile_MaterialStorage_GetDataBarcode]
	@BarcodeNo varchar(100)
as
begin
	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and AreaCode = 'TMP' and AddressCode = 'TMP' )
	begin
		raiserror('Lokasi stock sudah tidak berada di area temporary!', 16,1)
		return
	end

	if exists 
	(
		select 1 
		from StockDetail 
		where BarcodeNo = @BarcodeNo 
		and Qty > 0 
		and WarehouseCode in 
		(
			select Subcon_WH_Code from Trade_Master where Trade_Cls = '3'
		) 
	)
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	declare @refNo varchar(50) = (select RefNo from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)

	declare @listStatusReceiptFromParamBarcodes table (StatusReceipt varchar(50))
	insert into @listStatusReceiptFromParamBarcodes
	select distinct(StatusReceipt) StatusReceipt 
	From StockDetail where RefNo = @refNo and Qty > 0	

	declare @msg varchar(max)

	if (select count(1) from @listStatusReceiptFromParamBarcodes) > 1
	begin
		declare @x varchar(max) = (select STRING_AGG(StatusReceipt, ',') from @listStatusReceiptFromParamBarcodes)
		set @msg = 'Stock terdapat barcode dengan status berbeda (' + @x + ')'
		raiserror(@msg, 16,1)
		return
	end

	if exists (select 1 from @listStatusReceiptFromParamBarcodes where StatusReceipt not in ('OK', 'NG'))
	begin
		raiserror('Hanya Status Stock OK / NG yang bisa dipindahkan!', 16,1)
		return
	end

	SELECT sd.RefNo, sd.WarehouseCode, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.SublotNo, sd.Qty, sd.StatusReceipt
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
GO
