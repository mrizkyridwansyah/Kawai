
create   procedure [dbo].[sp_Wms_Robot_MoveTrolley]
	@RefNo varchar(50),
	@FromAddressCode varchar(25),
	@ToAddressCode varchar(25),
	@RobotCode varchar(50) = 'Robot'
as
begin
	insert into AMRMoveTrolley (TrolleyNo, FromAddressCode, ToAddressCode, RegisterDate)
	values (@RefNo, @FromAddressCode, @ToAddressCode, getdate())

	declare @StopPointCode varchar(25) = @ToAddressCode

	if not exists (select 1 from StockDetail where RefNo = @RefNo)
	begin
		--raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if isnull((select sum(Qty) from StockDetail where RefNo = @RefNo), 0) = 0
	begin
		--raiserror('Qty Stock sudah habis!', 16,1)
		return
	end

	--if not exists (select 1 from MS_Address where AddressCode = @AddressCode)
	--begin
	--	raiserror('Data Address tidak ditemukan!', 16,1)
	--	return
	--end

	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPointCode)
	begin
		raiserror('Data Stop Point tidak ditemukan!', 16,1)
		return
	end

	--if not exists (select 1 from MS_Address where AddressCode = @AddressCode and isnull(StopPointCode, '') = @StopPointCode)
	--begin
	--	raiserror('Address belum disetting Stop Point ini!', 16,1)
	--	return
	--end

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

	declare @AddressCode varchar(25) 

	select top 1 @AddressCode = AddressCode From MS_Address where StopPointCode = @StopPointCode

	DECLARE @ToWarehouseCode varchar(25), @ToAreaCode varchar(25), @ToAddressName varchar(max)
	select @ToWarehouseCode = a.WarehouseCode, @ToAreaCode = a.AreaCode, @ToAddressName = AddressName
	From MS_Address a
	left join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where AddressCode = @AddressCode

	insert into @tbl
	select 
		a.RefNo, 
		a.WarehouseCode, b.WarehouseName, a.AreaCode, a.AddressCode, a.ItemCode, a.BarcodeNo, a.LotNo, 
		@ToWarehouseCode, @ToAreaCode, @AddressCode, @ToAddressName, Qty
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
		'OUT', 'Robot Moving Trolley', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Robot Moving Trolley ke ' + isnull(ToAddressName, ''), RefNo, 
		getdate(), @RobotCode
	from @tbl

	exec sp_Wms_Stock_MovingRef @RefNo, @ToWarehouseCode, @ToAreaCode, @AddressCode, NULL, @RobotCode

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Robot Moving Trolley', RefNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Robot Moving Trolley dari ' + isnull(FromWarehouseName, ''), RefNo, 
		getdate(), @RobotCode
	from @tbl
end
