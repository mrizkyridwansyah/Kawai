


create   procedure [dbo].[sp_Wms_Receipt_Verify]
	@RefNo varchar(50),
	@Details tvp_ReceiptDetailVerify READONLY,
	@VerifiedBy varchar(25)
as
	DECLARE @msgError varchar(max)

	declare @tblReceipDetailBarcodes table (Id bigint, Barcode varchar(100), ReceiptId bigint, Warehouse varchar(25))
	insert into @tblReceipDetailBarcodes
	SELECT b.Id, b.BarcodeNo, b.ReceiptId, b.WarehouseCode FROM @Details a
	left join PartReceiptDetailBarcode b on a.ReceiptDetailBarcodeId = b.Id 

	IF EXISTS (SELECT 1 FROM @tblReceipDetailBarcodes WHERE Id is null)
	BEGIN
		declare @barcodes varchar(max) = (SELECT STRING_AGG(Barcode, ',') FROM @tblReceipDetailBarcodes WHERE Id is null)
		set @msgError = 'Data Receipt Barcode ('+isnull(@barcodes, '')+') tidak ditemukan!'
		RAISERROR(@msgError, 16, 1)
		RETURN
	END

	IF (SELECT count(distinct ReceiptId) FROM @tblReceipDetailBarcodes) > 1
	BEGIN
		set @msgError = 'Grouping Receipt Barcode harus dalam 1 DN yang sama!'
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

	declare @invalidBarcodes varchar(max) = 
	(
		select STRING_AGG(a.Barcode, ', ') From 
		(
			select dtl.ReceiptId, dtl.Warehouse, dtl.Barcode from @tblReceipDetailBarcodes dtl
		) a 
		left join 
		(
			select WarehouseCode from SS_UserWarehousePrivilege where UserID = @VerifiedBy and isnull(AllowAccess, 0) = 1
		) b on a.Warehouse = b.WarehouseCode
		where b.WarehouseCode is null
	)

	if isnull(@invalidBarcodes, '') <> ''
	begin
		declare @errors varchar(max) = 'User tidak memiliki hak akses ke warehouse dari salah satu barcode!'
		raiserror(@errors, 16, 1)
		return
	end

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

		DECLARE @ReceiptId bigint, @WarehouseCode varchar(25), @ItemCode varchar(25), @LotNo varchar(100), 
				@AreaCode varchar(25) = 'TMP', @AddressCode varchar(25) = 'TMP'

		SELECT 
			@ReceiptId = pr.ReceiptId, @WarehouseCode = isnull(po.WHTo, mi.WH_Code), @ItemCode = pr.ItemCode, @LotNo = pr.LotNo
		FROM 
		(
			select * From PartReceiptDetailBarcode WHERE Id = @Id
		) pr
		inner join PartReceiptDetail dtl on pr.ReceiptDetailId = dtl.Id
		inner join Item_Master mi on pr.ItemCode = mi.Item_Code
		left join PurchaseOrder_Master po on dtl.PONumber = po.PO_No

		UPDATE PartReceiptHeader SET StatusReceipt = 'PENDING', LastUpdate = GETDATE(), LastUser = @VerifiedBy WHERE Id = @ReceiptId

		EXEC sp_Wms_Stock_UpSertStockDetail @RefNo, @WarehouseCode, @AreaCode, @AddressCode, @ItemCode, @BarcodeNo, @LotNo, @QtyVerify, NULL, NULL, @VerifiedBy, 'HOLD', 'Vendor'
		EXEC sp_Wms_Stock_UpSertStockHeader @TransDate, @RefNo, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, @QtyVerify, NULL, 'R', @VerifiedBy

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



