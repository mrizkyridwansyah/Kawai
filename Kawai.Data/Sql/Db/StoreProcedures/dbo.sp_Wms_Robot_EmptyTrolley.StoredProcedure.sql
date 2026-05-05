CREATE   procedure [dbo].[sp_Wms_Robot_EmptyTrolley]
	@RefNo varchar(25)
AS
BEGIN
	if not exists (select 1 from MS_Trolley where TrolleyCode = @RefNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where RefNo = @RefNo)
	begin
		raiserror('Data Trolley sudah kosong!', 16,1)
		return
	end

	if isnull((select sum(Qty) from StockDetail where RefNo = @RefNo), 0) = 0
	begin
		raiserror('Data Trolley sudah kosong!', 16,1)
		return
	end

	declare @tbl table 
	(
		RefNo varchar(50), 
		ToRefNo varchar(50), 
		WarehouseCode varchar(50), 
		AreaCode varchar(25), 
		AddressCode varchar(25), 
		ItemCode varchar(25), 
		BarcodeNo varchar(50), 
		LotNo varchar(100), 
		Qty numeric(18,9),
		PickingNo varchar(100)
	)

	insert into @tbl
	select 
		a.RefNo, 'TR-TEMP' ToRefNo,
		a.WarehouseCode, a.AreaCode, a.AddressCode, a.ItemCode, a.BarcodeNo, a.LotNo, Qty, isnull(a.Picking_No, '')
	from StockDetail a
	where RefNo = @RefNo
	and isnull(qty,	0) > 0

	BEGIN TRY
		BEGIN TRANSACTION EmptyTrolleyTransaction

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
			RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
			QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'OUT', 'Robot Empty Trolley', RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
			ToRefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
			Qty, 'Robot Empty Trolley No ' + @RefNo, PickingNo, 
			getdate(), 'Robot'
		from @tbl

		declare @ToWarehouseCode varchar(25), @ToAreaCode varchar(25), @ToAddressCode varchar(25)
		select top 1 @ToWarehouseCode = WarehouseCode, @ToAreaCode = AreaCode, @ToAddressCode = AddressCode From @tbl

		exec sp_Wms_Stock_MovingRef @RefNo, @ToWarehouseCode, @ToAreaCode, @ToAddressCode, 'TR-TEMP', 'Robot'

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
			RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
			QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'IN', 'Robot Empty Trolley', ToRefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
			RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, 
			Qty, 'Robot Empty Trolley dari Trolley No ' + @RefNo, PickingNo, 
			getdate(), 'Robot'
		from @tbl

		COMMIT TRANSACTION EmptyTrolleyTransaction

		select top 1 @RefNo TrolleyNo, ToRefNo NewRefNo, PickingNo FROM @tbl
	end try
	begin catch
		ROLLBACK TRANSACTION EmptyTrolleyTransaction
		declare @msgErr varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msgErr, 16, 1)
		RETURN 
	end catch
END
