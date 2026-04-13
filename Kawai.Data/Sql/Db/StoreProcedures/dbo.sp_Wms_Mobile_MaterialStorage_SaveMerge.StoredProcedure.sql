SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Mobile_MaterialStorage_SaveMerge]
	@RefNo varchar(50),
	@AddressCode varchar(25),
	@ListBarcodes tvp_MaterialMergeStorage READONLY,
	@UserId varchar(25)
as
begin
	declare @msg varchar(max)

	if exists 
	(
		select 1 from @ListBarcodes a
		left join 
		(
			select * From StockDetail where Qty > 0
		) b on a.ItemCode = b.ItemCode and a.BarcodeNo = b.BarcodeNo and a.LotNo = b.LotNo
		where b.BarcodeNo is null
	)
	begin
		raiserror('Qty Stock sudah habis!', 16,1)
		return
	end

	if not exists (select 1 from MS_Address where AddressCode = @AddressCode)
	begin
		raiserror('Address tidak ditemukan!', 16,1)
		return
	end

	if exists 
	(
		select 1 from @ListBarcodes a
		inner join 
		(
			select * From StockDetail 
			where Qty > 0
			and WarehouseCode in 
			(
				select Subcon_WH_Code from Trade_Master where Trade_Cls = '3'
			) 		
		) b on a.ItemCode = b.ItemCode and a.BarcodeNo = b.BarcodeNo and a.LotNo = b.LotNo		
	)
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end


	DECLARE @ToWarehouseCode varchar(25), @ToAreaCode varchar(25), @ToAddressName varchar(max)
	select @ToWarehouseCode = a.WarehouseCode, @ToAreaCode = AreaCode, @ToAddressName = AddressName
	From MS_Address a
	left join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where AddressCode = @AddressCode

	if not exists (select 1 from SS_UserWarehousePrivilege where WarehouseCode = @ToWarehouseCode and UserID = @UserId and AllowAccess = 1)
	begin
		raiserror('User tidak memiliki hak akses ke address tersebut!', 16,1)
		return
	end

	declare @ngCls varchar(5) = 
	(
		select b.NG_Cls from MS_Address a
		inner join WareHouse_Master b on a.WarehouseCode = b.WH_Code
		where AddressCode = @AddressCode
	)

	declare @listStatusReceiptFromParamBarcodes table (StatusReceipt varchar(50))
	insert into @listStatusReceiptFromParamBarcodes
	select distinct(b.StatusReceipt) StatusReceipt from @ListBarcodes a
	inner join 
	(
		select ItemCode, BarcodeNo, LotNo, StatusReceipt From StockDetail where Qty > 0
	) b on a.ItemCode = b.ItemCode and a.BarcodeNo = b.BarcodeNo and a.LotNo = b.LotNo

	if (select count(1) from @listStatusReceiptFromParamBarcodes) > 1
	begin
		declare @x varchar(max) = (select STRING_AGG(StatusReceipt, ',') from @listStatusReceiptFromParamBarcodes)
		set @msg = 'Tidak boleh gabung barcode dengan status berbeda (' + @x + ')'
		raiserror(@msg, 16,1)
		return
	end

	declare @statusBarcodes varchar(50) = (select top 1 StatusReceipt from @listStatusReceiptFromParamBarcodes)
	if @statusBarcodes not in ('OK', 'NG')
	begin
		set @msg = 'Hanya Status Stock OK / NG yang bisa digabungkan!'
		raiserror(@msg, 16,1)
		return
	end

	if @statusBarcodes = 'NG' and @ngCls <> '01'
	begin
		set @msg = 'Stock yang NG tidak boleh dipindahkan ke warehouse GOOD'
		raiserror(@msg, 16,1)
		return
	end
	else if @statusBarcodes = 'OK' and @ngCls = '01'
	begin
		set @msg = 'Stock yang GOOD tidak boleh dipindahkan ke warehouse NG'
		raiserror(@msg, 16,1)
		return
	end

	declare @tbl table 
	(
		Urutan int,
		RefNo varchar(50), 
		FromWarehouseCode varchar(50), 
		FromWarehouseName varchar(max), 
		FromAreaCode varchar(25), 
		FromAddressCode varchar(25), 
		ItemCode varchar(25), 
		BarcodeNo varchar(50), 
		LotNo varchar(100), 
		SublotNo int, 
		ToWarehouseCode varchar(50), 
		ToAreaCode varchar(25), 
		ToAddressCode varchar(25), 
		ToAddressName varchar(max), 
		Qty numeric(18,9),
		InventoryQty numeric(18,9)
	)

	insert into @tbl
	select 
		ROW_NUMBER() over (order by stok.RefNo), 
		stok.RefNo, 
		stok.WarehouseCode, wh.WarehouseName, stok.AreaCode, stok.AddressCode, stok.ItemCode, stok.BarcodeNo, stok.LotNo, stok.SublotNo,
		@ToWarehouseCode, @ToAreaCode, @AddressCode, @ToAddressName,
		stok.Qty, stok.InventoryQty
	from @ListBarcodes a
	inner join 
	(
		select * From StockDetail where Qty > 0
	) stok on a.ItemCode = stok.ItemCode and a.BarcodeNo = stok.BarcodeNo and a.LotNo = stok.LotNo
	left join vw_WarehouseLine wh on wh.WarehouseCode = stok.WarehouseCode

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'OUT', 'Material Merge Storage Mobile', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		@RefNo, ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Material Merge Storage Mobile ke ' + isnull(ToAddressName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

	declare @i int = 1, @transDate date = getdate()
	while @i <= (select count(1) from @tbl)
	begin
		declare @ref varchar(100), @fromWH varchar(25), @fromArea varchar(25), @fromAddress varchar(25), 
				@item varchar(25), @lot varchar(100), @qty numeric(18, 9), @ivtQty numeric(18,9), @barcode varchar(50), @sublot int
		select 
			@ref = RefNo, @fromWH = FromWarehouseCode, @fromArea = FromAreaCode, @fromAddress = FromAddressCode, @item = ItemCode, @lot = LotNo,
			@qty = Qty, @ivtQty = InventoryQty, @barcode = BarcodeNo, @sublot = SublotNo
		from @tbl where Urutan = @i

		exec sp_Wms_Stock_UpSertStockDetail @ref, @fromWH, @fromArea, @fromAddress, @item, @barcode, @lot, 0, NULL, @sublot, @UserId, 'OK'
		exec sp_Wms_Stock_UpSertStockHeader @transDate, @ref, @fromWH, @fromArea, @item, @lot, @qty, NULL, 'S', @UserId

		exec sp_Wms_Stock_UpSertStockDetail @RefNo, @ToWarehouseCode, @ToAreaCode, @AddressCode, @item, @barcode, @lot, @qty, @ivtQty, @sublot, @UserId, 'OK'
		exec sp_Wms_Stock_UpSertStockHeader @transDate, @RefNo, @ToWarehouseCode, @ToAreaCode, @item, @lot, @qty, @ivtQty, 'R', @UserId

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
		'IN', 'Material Merge Storage Mobile', @RefNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		RefNo, FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Material Merge Storage Mobile dari ' + isnull(FromWarehouseName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

end
GO
