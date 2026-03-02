SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_Mobile_LoadingTrolley_ScanBarcode]
	@TrolleyNo varchar(50),
	@BarcodeNo varchar(50),
	@UserId varchar(25)
as
begin
	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	declare @pickingNo varchar(100) = (select top 1 RefNumber From PartMaterialRequestDetail where Trolley_No = @TrolleyNo order by RegisterDate desc)

	if @pickingNo is null
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data barcode tidak ditemukan / sudah habis!', 16,1)
		return
	end

	if not exists (select 1 From StockDetail where Picking_No = @pickingNo and Qty > 0)
	begin
		raiserror('Data stock picking tidak ditemukan!', 16,1)
		return
	end

	declare @RefNo varchar(25), @WarehouseCode varchar(25), @AreaCode varchar(25), @AddressCode varchar(25), @StokPickingNo varchar(100), @StokStopPoint varchar(25)
	select 
		@RefNo = RefNo, 
		@WarehouseCode = sd.WarehouseCode, @AreaCode = sd.AreaCode, @AddressCode = sd.AddressCode, 
		@StokPickingNo = sd.Picking_No, @StokStopPoint = ma.StopPointCode
	from StockDetail sd
	left join MS_Address ma on sd.AddressCode = ma.AddressCode
	where sd.BarcodeNo = @BarcodeNo and sd.Qty > 0

	if @RefNo = @TrolleyNo
	begin
		raiserror('Data stock sudah diloading ke trolley!', 16,1)
		return		
	end

	if isnull(@StokPickingNo, '') <> @pickingNo
	begin
		raiserror('Data barcode bukan untuk loading trolley ini!', 16,1)
		return		
	end

	declare @currentStopPoint varchar(25) = 
	(
		select top 1 Stop_Point 
		From PartMaterialRequestSendRobotDetail 
		where RequestSendID = @pickingNo and [Status] = 0 order by Pickup_Seq
	)

	declare @errMsg varchar(max) = ''
	if isnull(@StokStopPoint, @currentStopPoint) <> @currentStopPoint
	begin
		declare @spName varchar(100) = (select [Description] from MS_StopPoint where StopPointCode = @currentStopPoint)
		set @errMsg = 'Stock barcode tidak ada di stop point ('+@spName+')!'

		raiserror(@errMsg, 16,1)
		return
	end

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'OUT', 'Loading Trolley Mobile', RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
		@TrolleyNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Loading Trolley ' + isnull(@TrolleyNo, ''), RefNo, 
		getdate(), @UserId
	from StockDetail stok 
	where stok.RefNo = @RefNo and Qty > 0

	exec sp_Wms_Stock_MovingRef @RefNo, @WarehouseCode, @AreaCode, @AddressCode, @TrolleyNo, @UserId

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, RefNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
		RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
		QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Loading Trolley Mobile', @TrolleyNo, 
		WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
		RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Loading Trolley Mobile dari ' + isnull(@RefNo, ''), RefNo, 
		getdate(), @UserId
	from StockDetail stok 
	where stok.RefNo = @TrolleyNo and Qty > 0
end
GO
