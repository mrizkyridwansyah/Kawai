SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_WMS_Closing]
	@IvtYear int,
	@IvtMonth int,
	@UserId varchar(25)
as
begin
	declare @NewPeriod date = datefromparts(@IvtYear, @IvtMonth, 1)
	declare @msgError varchar(max)

	IF EXISTS (SELECT 1 FROM Inventory_Control WHERE Inventory_Year = @IvtYear AND Inventory_Month = @IvtMonth)
	BEGIN
		RAISERROR('Periode ini sudah ditutup!',16,1)
		RETURN
	END

	DECLARE @LastPeriodClosing date = 
	(SELECT TOP 1 datefromparts(Inventory_Year, Inventory_Month, 1) FROM Inventory_Control ORDER BY Inventory_Year DESC, Inventory_Month DESC)

	IF @NewPeriod = DATEADD(month, 1, @LastPeriodClosing) and @LastPeriodClosing is not null
	BEGIN
		set @msgError = 'Periode Terakhir adalah ' + format(@LastPeriodClosing, 'MMM yyyy')
		RAISERROR(@msgError,16,1)
		RETURN
	END

	-- UPDATE DULU INVENTORY QTY SESUAI DARI STOCK OPNAME
	update sd set InventoryQty = so.InventoryQty
	From StockDetail sd
	inner join StockOpname so 
	on 
		sd.RefNo = so.RefNo 
		and sd.WarehouseCode = so.WarehouseCode 
		and sd.AreaCode = so.AreaCode 
		and sd.AddressCode = so.AddressCode 
		and sd.ItemCode = so.ItemCode 
		and sd.BarcodeNo = so.BarcodeNo 
		and sd.LotNo = so.LotNo 

	-- UPDATE DULU TMINVENTORY QTY SESUAI DARI STOCK OPNAME
	update sh set TMInventory = sd.TMInventoryQty
	from StockHeader sh 
	inner join 
	(
		SELECT RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, sum(InventoryQty) TMInventoryQty FROM StockDetail 
		group by RefNo, WarehouseCode, AreaCode, ItemCode, LotNo
	) sd on 
		sd.RefNo = sh.RefNo 
		and sd.WarehouseCode = sh.WarehouseCode 
		and sd.AreaCode = sh.AreaCode 
		and sd.ItemCode = sh.ItemCode 
		and sd.LotNo = sh.LotNo 

	-- MOVE KE STOCK HISTORY
	insert into StockHeaderHistory
	(
		[Period], RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, PreMonth, Receipt, Supply, LossReject, [Current], Inventory, 
		Reason, Adjustment, RegisterUpdate, RegisterUser
	)	
	select 
		FORMAT(@NewPeriod, 'yyyyMM'), RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, LMPreMonth, LMReceipt, LMSupply, LMLossReject, LMCurrent, LMInventory, 
		LMReason, Adjustment, getdate(), @UserId
	From StockHeader

	insert into StockDetailHistory
	(
		[Period], RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, 
		ExpiredDate, ProductionDate, ReceiptDate, Supplier, PrintCls, DisposalCls, StatusReceipt, 
		RegisterDate, RegisterUser, Lastupdate, LastUser, ClosingDate, ClosingUser
	)	
	select 
		FORMAT(@NewPeriod, 'yyyyMM'), RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, 
		ExpiredDate, ProductionDate, ReceiptDate, Supplier, PrintCls, DisposalCls, StatusReceipt, 
		RegisterDate, RegisterUser, Lastupdate, LastUser, GETDATE(), @UserId
	From StockDetail sd

	-- UPDATE StockHeader
	update sh 
	set 
		LMPreMonth = TMPreMonth,
		LMReceipt = TMReceipt,
		LMSupply = TMSupply,
		LMLossReject = TMLossReject,
		LMCurrent = TMCurrent,
		LMInventory = ISNULL(TMInventory, 0),
		TMPreMonth = ISNULL(TMInventory, 0),
		TMReceipt = NMReceipt,
		TMSupply = NMSupply,
		TMLossReject = NMLossReject,
		TMInventory = NULL,
		TMCurrent = (ISNULL(TMInventory, 0) + ISNULL(NMReceipt, 0)) - (ISNULL(NMSupply, 0) + ISNULL(NMLossReject, 0)),
		NMPreMonth = (ISNULL(TMInventory, 0) + ISNULL(NMReceipt, 0)) - (ISNULL(NMSupply, 0) + ISNULL(NMLossReject, 0)),
		NMReceipt = 0,
		NMSupply = 0,
		NMLossReject = 0,
		NMCurrent = (ISNULL(TMInventory, 0) + ISNULL(NMReceipt, 0)) - (ISNULL(NMSupply, 0) + ISNULL(NMLossReject, 0)),
		NMInventory = NULL
	from StockHeader sh 

	-- HAPUS StockHeader yg 
	DELETE FROM StockHeader WHERE ISNULL(LMInventory, 0) = 0

	-- HAPUS StockDetail yg GA DI SO
	DELETE FROM StockDetail WHERE ISNULL(InventoryQty, 0) = 0

	-- UPDATE StockDetail QTY SESUAI QTY SO & QTY SO DI SET JADI NULL
	UPDATE StockDetail SET Qty = InventoryQty, InventoryQty = NULL 
	WHERE ISNULL(InventoryQty, 0) > 0 AND ISNULL(Qty, 0) > 0

	insert into Inventory_Control (Inventory_Year, Inventory_Month, Fix_Cls, ClosingDate)
	values (@IvtYear, @IvtMonth, '1', getdate())

	DELETE FROM StockOpname
end
GO
