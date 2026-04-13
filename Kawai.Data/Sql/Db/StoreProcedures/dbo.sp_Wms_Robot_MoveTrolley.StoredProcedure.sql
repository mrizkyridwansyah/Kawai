SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_Robot_MoveTrolley]
	@RefNo varchar(50),
	@AreaCode varchar(25),
	@RobotCode varchar(50)
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

	if not exists (select 1 from MS_Area where AreaCode = @AreaCode)
	begin
		raiserror('Address tidak ditemukan!', 16,1)
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
		ToAreaName varchar(max), 
		ToAddressCode varchar(25), 
		Qty numeric(18,9)
	)

	DECLARE @ToWarehouseCode varchar(25), @ToAreaName varchar(max)
	select @ToWarehouseCode = a.WarehouseCode, @ToAreaName = AreaName
	From MS_Area a
	left join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where AreaCode = @AreaCode

	insert into @tbl
	select 
		a.RefNo, 
		a.WarehouseCode, b.WarehouseName, a.AreaCode, a.AddressCode, a.ItemCode, a.BarcodeNo, a.LotNo, 
		@ToWarehouseCode, @AreaCode, @ToAreaName, 'TMP',
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
		'OUT', 'Robot Moving Trolley', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Robot Moving Trolley ke ' + isnull(ToAreaName, ''), RefNo, 
		getdate(), @RobotCode
	from @tbl

	exec sp_Wms_Stock_MovingRef @RefNo, @ToWarehouseCode, @AreaCode, 'TMP', NULL, @RobotCode

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
GO
