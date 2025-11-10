SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_Receipt_Verify]
	@Id bigint,
	@BarcodeNo varchar(50),
	@Qty numeric(18,9),
	@QtyVerify numeric(18,9),
	@VerifiedBy varchar(25)
as
	IF NOT EXISTS (SELECT 1 FROM PartReceiptDetailBarcode WHERE Id = @Id)
	BEGIN
		RAISERROR('Data Receipt Barcode tidak ditemukan', 16, 1)
		RETURN
	END

	IF EXISTS (SELECT 1 FROM StockDetail WHERE BarcodeNo = @BarcodeNo)
	BEGIN
		RAISERROR('Barcode sudah diterima', 16, 1)
		RETURN
	END

	UPDATE PartReceiptDetailBarcode SET Qty = @QtyVerify, VerifiedBy = @VerifiedBy, IsVerified = 1, VerifiedDate = GETDATE() WHERE Id = @Id

	DECLARE @ReceiptId bigint = (SELECT ReceiptId FROM PartReceiptDetailBarcode WHERE Id = @Id)
	UPDATE PartReceiptHeader SET StatusReceipt = 'PENDING', LastUpdate = GETDATE(), LastUser = @VerifiedBy WHERE Id = @ReceiptId

	DECLARE @TransDate date = GETDATE()
	DECLARE @WarehouseCode varchar(25), @ItemCode varchar(25), @LotNo varchar(100),
			@AreaCode varchar(25) = 'TMP', @AddressCode varchar(25) = 'TMP'

	SELECT @ItemCode = ItemCode, @LotNo = LotNo fROM StockDetail WHERE BarcodeNo = @BarcodeNo AND Qty > 0
	SELECT @WarehouseCode = WH_Code fROM Item_Master WHERE Item_Code = @ItemCode

	EXEC sp_Wms_Stock_UpSertStockDetail @WarehouseCode, @AreaCode, @AddressCode, @ItemCode, @BarcodeNo, @LotNo, @QtyVerify, NULL, NULL, @VerifiedBy
	EXEC sp_Wms_Stock_UpSertStockMaster @TransDate, @WarehouseCode, @AddressCode, @ItemCode, @LotNo, @QtyVerify, NULL, 'R', @VerifiedBy

	insert into ReceiptSupplyHistory 
	(
		[Status], ProcessMenu, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, QtyTrans, Remarks, ReferenceNo, LogDate, UserID
	)
	select 
		'IN', 'Receipt Mobile', @WarehouseCode, @AreaCode, @AddressCode, ItemCode, @BarcodeNo, @LotNo, @QtyVerify, 'Receipt Barcode Mobile', @BarcodeNo, 
		getdate(), @VerifiedBy
	from PartReceiptDetailBarcode
	where Id = @Id
GO
