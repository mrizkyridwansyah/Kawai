SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Mobile_LoadingTrolley_Save]
	@TrolleyNo varchar(50),
	@RequestDetailId bigint,
	@UserId varchar(25)
as
begin
	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists 
	(
		select * from 
		(
			select * From PartMaterialRequestItemDetail where RequestDetailID = @RequestDetailId
		) pmrid
		inner join PartMaterialRequestItemDetailScan scan on pmrid.IDSeq = scan.IDSeq
		inner join 
		(
			select * From StockDetail where Qty > 0
		) sd on scan.BarcodeNo = sd.BarcodeNo and scan.LotNo = sd.LotNo and sd.ItemCode = scan.ItemCode
	)
	begin
		raiserror('Data Stock tidak ditemukan / sudah habis!', 16,1)
		return
	end

	if exists (select 1 from StockDetail where RefNo = @TrolleyNo and Qty > 0)
	begin
		raiserror('Barcode sudah ada didalam troli!', 16,1)
		return
	end

	declare @TrolleyReqDetail varchar(50) = (select Trolley_No from PartMaterialRequestDetail where RequestDetailID = @RequestDetailId)

	if isnull(@TrolleyReqDetail, '') <> @TrolleyNo
	begin
		raiserror('Troli tidak valid!', 16,1)
		return
	end

	declare @tbl table 
	(
		Urutan int,
		RefNo varchar(50), 
		FromWarehouseCode varchar(50), 
		FromAreaCode varchar(25), 
		FromAddressCode varchar(25), 
		ItemCode varchar(25), 
		BarcodeNo varchar(50), 
		LotNo varchar(100), 
		SublotNo int, 
		ToWarehouseCode varchar(50), 
		ToAreaCode varchar(25), 
		ToAddressCode varchar(25), 
		Qty numeric(18,9),
		InventoryQty numeric(18,9)
	)

	insert into @tbl
	select 
		ROW_NUMBER() over (order by stok.RefNo), 
		stok.RefNo, 
		stok.WarehouseCode, stok.AreaCode, stok.AddressCode, 
		stok.ItemCode, stok.BarcodeNo, stok.LotNo, stok.SublotNo,
		stok.WarehouseCode, stok.AreaCode, stok.AddressCode,
		stok.Qty, stok.InventoryQty
	from 		
	(
		select * From PartMaterialRequestItemDetail where RequestDetailID = @RequestDetailId
	) pmrid
	inner join PartMaterialRequestItemDetailScan scan on pmrid.IDSeq = scan.IDSeq
	inner join 
	(
		select * From StockDetail where Qty > 0
	) stok on scan.ItemCode = stok.ItemCode and scan.BarcodeNo = stok.BarcodeNo and scan.LotNo = stok.LotNo

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'OUT', 'Loading Trolley Mobile', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		@TrolleyNo, ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Loading Trolley ' + isnull(@TrolleyNo, ''), RefNo, 
		getdate(), @UserId
	from @tbl

	declare @i int = 1, @transDate date = getdate()
	while @i <= (select count(1) from @tbl)
	begin
		declare @ref varchar(100), @warehouse varchar(25), @area varchar(25), @address varchar(25), 
				@item varchar(25), @lot varchar(100), @qty numeric(18, 9), @ivtQty numeric(18,9), @barcode varchar(50), @sublot int
		select 
			@ref = RefNo, @warehouse = FromWarehouseCode, @area = FromAreaCode, @address = FromAddressCode, @item = ItemCode, @lot = LotNo,
			@qty = Qty, @ivtQty = InventoryQty, @barcode = BarcodeNo, @sublot = SublotNo
		from @tbl where Urutan = @i

		exec sp_Wms_Stock_UpSertStockDetail @ref, @warehouse, @area, @address, @item, @barcode, @lot, 0, NULL, @sublot, @UserId, 'OK'
		exec sp_Wms_Stock_UpSertStockHeader @transDate, @ref, @warehouse, @area, @item, @lot, @qty, NULL, 'S', @UserId

		exec sp_Wms_Stock_UpSertStockDetail @TrolleyNo, @warehouse, @area, @address, @item, @barcode, @lot, @qty, @ivtQty, @sublot, @UserId, 'OK'
		exec sp_Wms_Stock_UpSertStockHeader @transDate, @TrolleyNo, @warehouse, @area, @item, @lot, @qty, @ivtQty, 'R', @UserId

		set @i += 1
	end

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Loading Trolley Mobile', @TrolleyNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		RefNo, FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Loading Trolley Mobile dari ' + isnull(RefNo, ''), RefNo, 
		getdate(), @UserId
	from @tbl
end
GO
