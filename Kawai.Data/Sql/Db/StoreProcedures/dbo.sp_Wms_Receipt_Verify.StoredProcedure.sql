SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_Receipt_Verify]
	@RefNo varchar(50),
	@Details tvp_ReceiptDetailVerify READONLY,
	@VerifiedBy varchar(25)
as
	DECLARE @msgError varchar(max)

	IF EXISTS 
	(
		SELECT 1 FROM @Details a
		left join PartReceiptDetailBarcode b on a.ReceiptDetailBarcodeId = b.Id 
		WHERE b.Id is null
	)
	BEGIN
		declare @barcodes varchar(max) = 
		(
			SELECT STRING_AGG(a.BarcodeNo, ',') FROM @Details a
			left join PartReceiptDetailBarcode b on a.ReceiptDetailBarcodeId = b.Id 
			WHERE b.Id is null		
		)
		set @msgError = 'Data Receipt Barcode ('+isnull(@barcodes, '')+') tidak ditemukan!'
		RAISERROR(@msgError, 16, 1)
		RETURN
	END

	IF EXISTS 
	(
		SELECT 1 FROM @Details a
		inner join StockDetail b on a.BarcodeNo = b.BarcodeNo
	)
	BEGIN
		declare @barcodesReceipts varchar(max) = 
		(
			select STRING_AGG(res.BarcodeNo, ',')
			from
			(
				SELECT distinct a.BarcodeNo FROM @Details a
				inner join StockDetail b on a.BarcodeNo = b.BarcodeNo 
			) res
		)
		set @msgError = 'Barcode ('+isnull(@barcodesReceipts, '')+') sudah di grouping!'

		RAISERROR(@msgError, 16, 1)
		RETURN
	END

	DECLARE @TransDate date = GETDATE()

	declare @i int = 1
	while @i <= (select count(1) from @Details)
	begin
		declare @Id bigint, @BarcodeNo varchar(50), @Qty numeric(18,9), @QtyVerify numeric(18,9)
		select @Id = ReceiptDetailBarcodeId, @BarcodeNo = BarcodeNo, @Qty = Qty, @QtyVerify = QtyVerify 
		From 
		(
			select ROW_NUMBER() over (order by ReceiptDetailBarcodeId) Urutan, * From @Details
		) res where res.Urutan = @i

		UPDATE PartReceiptDetailBarcode SET Qty = @QtyVerify, VerifiedBy = @VerifiedBy, IsVerified = 1, VerifiedDate = GETDATE() WHERE Id = @Id

		DECLARE @ReceiptId bigint = (SELECT ReceiptId FROM PartReceiptDetailBarcode WHERE Id = @Id)
		UPDATE PartReceiptHeader SET StatusReceipt = 'PENDING', LastUpdate = GETDATE(), LastUser = @VerifiedBy WHERE Id = @ReceiptId

		DECLARE @WarehouseCode varchar(25), @ItemCode varchar(25), @LotNo varchar(100),
				@AreaCode varchar(25) = 'TMP', @AddressCode varchar(25) = 'TMP'

		SELECT @ItemCode = ItemCode, @LotNo = LotNo fROM PartReceiptDetailBarcode WHERE Id = @Id
		SELECT @WarehouseCode = WH_Code fROM Item_Master WHERE Item_Code = @ItemCode

		EXEC sp_Wms_Stock_UpSertStockDetail @RefNo, @WarehouseCode, @AreaCode, @AddressCode, @ItemCode, @BarcodeNo, @LotNo, @QtyVerify, NULL, NULL, @VerifiedBy, 'HOLD'
		EXEC sp_Wms_Stock_UpSertStockHeader @TransDate, @RefNo, @WarehouseCode, @AddressCode, @ItemCode, @LotNo, @QtyVerify, NULL, 'R', @VerifiedBy

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'IN', 'Receipt Mobile', @RefNo, @WarehouseCode, @AreaCode, @AddressCode, ItemCode, @BarcodeNo, @LotNo, @QtyVerify, 'Receipt Barcode Mobile', @BarcodeNo, 
			getdate(), @VerifiedBy
		from PartReceiptDetailBarcode
		where Id = @Id

		set @i += 1
	end



GO
