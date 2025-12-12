USE [Kawaii]
GO
/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_AssignStorage_Save]    Script Date: 12/12/2025 9:57:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_Wms_Mobile_AssignStorage_Save]
	@RefNo varchar(50),
	@AddressCode varchar(25),
	@UserId varchar(25)
as
begin
	if not exists (select 1 from StockDetail where RefNo = @RefNo)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where RefNo = @RefNo and Qty > 0 and StatusReceipt = 'OK')
	begin
		raiserror('Status Stock belum OK!', 16,1)
		return
	end

	if isnull((select sum(Qty) from StockDetail where RefNo = @RefNo), 0) = 0
	begin
		raiserror('Qty Stock sudah habis!', 16,1)
		return
	end

	if not exists (select 1 from MS_Address where AddressCode = @AddressCode)
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
		ToAddressCode varchar(25), 
		ToAddressName varchar(max), 
		Qty numeric(18,9)
	)

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
		'OUT', 'Assign Storage Mobile', RefNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Assign Storage Mobile ke ' + isnull(ToAddressName, ''), RefNo, 
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
		'IN', 'Assign Storage Mobile', RefNo, 
		ToWarehouseCode, ToAreaCode, ToAddressCode, ItemCode, BarcodeNo, LotNo, 
		FromWarehouseCode, FromAreaCode, FromAddressCode, ItemCode, BarcodeNo, LotNo, 
		Qty, 'Assign Storage Mobile dari ' + isnull(FromWarehouseName, ''), RefNo, 
		getdate(), @UserId
	from @tbl
end
GO
