SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Mobile_LoadingTrolley_GetDataBarcode]
	@TrolleyNo varchar(50),
	@BarcodeNo varchar(50)
as
begin
	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan / sudah habis!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and StatusReceipt = 'OK')
	begin
		raiserror('Data Stock belum OK!', 16,1)
		return
	end

	--if exists (select 1 from StockDetail where RefNo = @TrolleyNo and BarcodeNo = @BarcodeNo and Qty > 0)
	--begin
	--	raiserror('Barcode sudah ada didalam troli!', 16,1)
	--	return
	--end

	declare @TrolleyReqDetail varchar(50) = 
	(
		select pmrd.Trolley_No from
		(
			select * From PartMaterialRequestItemDetailScan where BarcodeNo = @BarcodeNo
		) scan
		inner join PartMaterialRequestItemDetail pmrid on scan.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
	)

	if isnull(@TrolleyReqDetail, '') <> @TrolleyNo
	begin
		raiserror('Troli tidak valid!', 16,1)
		return
	end

	declare @idSeq bigint = (select IDSeq From PartMaterialRequestItemDetailScan where BarcodeNo = @BarcodeNo)

	select 
		scan.ItemCode, mi.Item_Name ItemName, scan.BarcodeNo, scan.LotNo, scan.Qty CurrentQty, 0 SublotNo
	from PartMaterialRequestItemDetailScan scan
	inner join PartMaterialRequestItemDetail pmrid on scan.IDSeq = pmrid.IDSeq
	inner join Item_Master mi on scan.ItemCode = mi.Item_Code
	where scan.IDSeq = @idSeq
end
GO
