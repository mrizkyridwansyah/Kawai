SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_MaterialStorage_Save]
	@RefNo varchar(50),
	@AddressCode varchar(25),
	@BarcodeNo varchar(100),
	@UserId varchar(25)
as
begin
	if not exists (select 1 from StockDetail where RefNo = @RefNo)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if isnull((select sum(Qty) from StockDetail where RefNo = @RefNo), 0) = 0
	begin
		raiserror('Qty Stock sudah habis!', 16,1)
		return
	end

	if exists 
	(
		select 1 
		from StockDetail 
		where RefNo = @RefNo
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

	if not exists (select 1 from MS_Address where AddressCode = @AddressCode)
	begin
		raiserror('Address tidak ditemukan!', 16,1)
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

	declare @listStatusReceiptFromParamBarcodes table (StatusReceipt varchar(50))
	insert into @listStatusReceiptFromParamBarcodes
	select distinct(StatusReceipt) StatusReceipt 
	From StockDetail where RefNo = @RefNo and Qty > 0	

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

	declare @ngCls varchar(5) = 
	(
		select b.NG_Cls from MS_Address a
		inner join WareHouse_Master b on a.WarehouseCode = b.WH_Code
		where AddressCode = @AddressCode
	)

	declare @statusBarcodes varchar(50) = (select top 1 StatusReceipt from @listStatusReceiptFromParamBarcodes)
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
		RefNo varchar(50), 
		FromWarehouseCode varchar(50), 
		FromWarehouseName varchar(max), 
		FromAreaCode varchar(25), 
		FromAddressCode varchar(25), 
		ItemCode varchar(25), 
		BarcodeNo varchar(50), 
		LotNo varchar(100), 
		ToWarehouseCode varchar(50), 
		ToAreaCode varchar(25), 
		ToAddressCode varchar(25), 
		ToAddressName varchar(max), 
		Qty numeric(18,9)
	)

	insert into @tbl
	select 
		a.RefNo, 
		a.WarehouseCode, b.WarehouseName, a.AreaCode, a.AddressCode, a.ItemCode, a.BarcodeNo, a.LotNo, 
		@ToWarehouseCode, @ToAreaCode, @AddressCode, @ToAddressName,
		Qty
	from StockDetail a
	left join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where RefNo = @RefNo
	and isnull(qty,	0) > 0

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'OUT', 'Material Storage Mobile', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Material Storage Mobile ke ' + isnull(ToAddressName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

	exec sp_Wms_Stock_MovingRef @RefNo, @ToWarehouseCode, @ToAreaCode, @AddressCode, NULL, @UserId

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Material Storage Mobile', RefNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Material Storage Mobile dari ' + isnull(FromWarehouseName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

	-- ubah status nya jadi COMPLETE biar ilang dari andon
	update prh 
	set  
		StatusReceipt = 'COMPLETE', LastUpdate = getdate(), LastUser = @UserId 
	From PartReceiptHeader prh 
	inner join 
	(
		select distinct x.ReceiptId From PartReceiptDetailBarcode x
		inner join @tbl y on x.ItemCode = y.ItemCode and x.BarcodeNo = y.BarcodeNo and x.LotNo = y.LotNo 
	) dtl on prh.Id = dtl.ReceiptId
	where StatusReceipt IN ('OK', 'NG')
end
GO
