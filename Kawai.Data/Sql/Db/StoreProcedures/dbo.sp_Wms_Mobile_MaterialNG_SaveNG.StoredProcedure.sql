SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_MaterialNG_SaveNG]
	@BarcodeNo varchar(100),
	@QtyNG numeric(18,9),
	@UserId varchar(25)
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo and isnull(IsVerified, 0) = 1)
	begin
		raiserror('Data Barcode belum diterima!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and StatusReceipt = 'OK')
	begin
		raiserror('Status Barcode belum OK', 16, 1)
		return
	end

	declare @ReceiptId bigint, @PONumber varchar(100), @ItemCode varchar(25), @Qty numeric(18,9)
	select @ReceiptId = ReceiptId, @PONumber = PONumber, @ItemCode = ItemCode, @Qty = Qty
	From PartReceiptDetailBarcode 
	where BarcodeNo = @BarcodeNo

	IF @QtyNG > @Qty
	BEGIN
		raiserror('Qty NG tidak boleh melebihi Qty Receipt!', 16,1)
		return
	END

	if exists 
	(
		select 1 from IQC_SamplingBarcodeDetail dtl
		inner join IQC_Inspection_Header hd on dtl.InspectionID = hd.InspectionID 
		where hd.PO_Number = @PONumber 
		and hd.ItemCode = @ItemCode 
		and hd.Soruce = 'Material NG' 
		and hd.StatusQC = 'CONFIRMED'
	)
	begin
		raiserror('Material NG dari PO barcode ini sudah diconfirm!', 16,1)
		return
	end

	declare @InspectionId bigint = (select InspectionID from IQC_Inspection_Header where PO_Number = @PONumber and ItemCode = @ItemCode and Soruce = 'Material NG')

	begin transaction ngTransaction
	begin try
		if not exists (select 1 from IQC_Inspection_Header where PO_Number = @PONumber and ItemCode = @ItemCode and Soruce = 'Material NG')
		begin
			insert into IQC_Inspection_Header 
			(
				PO_Number
				, ReceiptNo
				, SupplierCode
				, ItemCode
				, ItemName
				, InspectionDate
				, InspectorID
				, InspectionResult
				, Remarks
				, RegisterDate
				, LastUpdate
				, TotalQtySample
				, Soruce
				, StatusQC
			)
			select 
				a.PONumber,
				b.ReceiptNo,
				b.SupplierCode,
				a.ItemCode,
				mi.Item_Name,
				getdate(),
				@UserId,
				null,
				'',
				getdate(),
				null,
				0,
				'Material NG',
				'NEW'
			From PartReceiptDetailBarcode a
			inner join PartReceiptHeader b on a.ReceiptId = b.Id
			inner join Item_Master mi on a.ItemCode = mi.Item_Code
			where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo

			SET @InspectionId = (select SCOPE_IDENTITY())
		end

		if not exists (select 1 from IQC_SamplingBarcodeDetail where InspectionID = @InspectionId and BarcodeNo = @BarcodeNo)
		begin
			insert into IQC_SamplingBarcodeDetail 
			(
				InspectionID
				, BarcodeNo
				, CurrentStock
				, SampleQTY
				, RegisterDate
				, RegisterUser
			)
			VALUES
			(
				@InspectionId,
				@BarcodeNo,
				@Qty,
				@QtyNG,
				getdate(),
				@UserId
			)
		end
		else
		begin
			update IQC_SamplingBarcodeDetail set SampleQTY = @QtyNG where InspectionID = @InspectionId and BarcodeNo = @BarcodeNo
		end

		UPDATE StockDetail SET StatusReceipt = 'HOLD', Lastupdate = GETDATE(), LastUser = @UserId WHERE BarcodeNo = @BarcodeNo
				
		select @InspectionId

		commit transaction ngTransaction
	end try
	begin catch
		rollback transaction ngTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
GO
