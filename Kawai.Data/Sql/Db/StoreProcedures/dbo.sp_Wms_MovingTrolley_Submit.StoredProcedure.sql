create   procedure sp_Wms_TransferTrolley_Submit
	@RequestNo varchar(100),
	@TrolleyNo varchar(50),
	@StopPoint varchar(25),
	@UserId varchar(25)
as
begin
	if not exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16, 1)
		return
	end

	if not exists (select 1 from StockDetail where RefNo = @TrolleyNo and Qty > 0)
	begin
		raiserror('Stock Trolley kosong!', 16, 1)
		return
	end

	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPoint)
	begin
		raiserror('Data Stop Point tidak ditemukan!', 16, 1)
		return
	end

	if not exists (select 1 from MS_StopPoint where StopPointCode = @StopPoint and IsActive = 1)
	begin
		raiserror('Data Stop Point tidak aktif!', 16, 1)
		return
	end

	declare @msg varchar(max) = ''

	if not exists (select 1 from StockDetail where RefNo = @TrolleyNo and isnull(Picking_No, '') = @RequestNo and Qty > 0)
	begin
		set @msg = 'Stock Trolley bukan milik Request No ' + @RequestNo + '!'
		raiserror(@msg, 16, 1)
		return
	end

	declare @line varchar(25), @ws varchar(25)
	select 
		top 1 @line= hd.LineCode, @ws = dtl.WorkStationCode
	from PartMaterialRequestDetail dtl
	inner join PartMaterialRequestHeader hd on dtl.RequestID = hd.RequestID
	inner join Manufacture_Line ml on hd.LineCode = ml.Line_Code
	inner join MS_WorkStation ws on dtl.WorkStationCode = ws.WorkStationCode
	where RefNumber = @RequestNo

	declare @stopPointName varchar(100) = (select [Description] From MS_StopPoint where StopPointCode = @StopPoint and IsActive = 1)

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
		a.WarehouseCode, b.WarehouseName, a.AreaCode, a.AddressCode, 
		a.ItemCode, a.BarcodeNo, a.LotNo, 
		@line, @ws, @StopPoint, @stopPointName, Qty
	from StockDetail a
	left join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where RefNo = @TrolleyNo
	and isnull(qty,	0) > 0

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'OUT', 'Mobile Moving Trolley', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Mobile Moving Trolley ke ' + isnull(ToAddressName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

	exec sp_Wms_Stock_MovingRef @TrolleyNo, @line, @ws, @StopPoint, NULL, @UserId

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Mobile Moving Trolley', RefNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Mobile Moving Trolley dari ' + isnull(FromWarehouseName, ''), RefNo, 
		getdate(), @UserId
	from @tbl

end


