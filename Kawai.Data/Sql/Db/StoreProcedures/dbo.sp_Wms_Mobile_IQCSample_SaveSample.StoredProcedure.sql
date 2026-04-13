SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_IQCSample_SaveSample]
	@ReceiptId bigint,
	@BarcodeNo varchar(100),
	@QtySample numeric(18,9),
	@UserId varchar(25)
as
begin
	if exists 
	(
		select 1 from IQC_SamplingBarcodeDetail dtl
		inner join IQC_Inspection_Header hd on dtl.InspectionID = hd.InspectionID 
		where dtl.BarcodeNo = @BarcodeNo and hd.Soruce = 'Incoming Material'
	)
	begin
		raiserror('Barcode sudah diinput sampel', 16,1)
		return
	end

	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo and isnull(IsVerified, 0) = 1)
	begin
		raiserror('Data Barcode belum diterima!', 16,1)
		return
	end

	declare @PONumber varchar(100), @ItemCode varchar(25), @Qty numeric(18,9), @ReceiptNo varchar(100)
	select @PONumber = a.PONumber, @ItemCode = a.ItemCode, @Qty = a.Qty, @ReceiptNo = b.ReceiptNo
	From PartReceiptDetailBarcode a 
	inner join PartReceiptHeader b on a.ReceiptId = b.Id
	where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo

	IF @QtySample > @Qty
	BEGIN
		raiserror('Qty Sample tidak boleh melebihi Qty Receipt!', 16,1)
		return
	END

	declare @InspectionId bigint = (select InspectionID from IQC_Inspection_Header where PO_Number = @PONumber and ReceiptNo = @ReceiptNo and ItemCode = @ItemCode and Soruce = 'Incoming Material')

	begin transaction sampleTransaction
	begin try
		if not exists (select 1 from IQC_Inspection_Header where PO_Number = @PONumber and ReceiptNo = @ReceiptNo and ItemCode = @ItemCode and Soruce = 'Incoming Material')
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
				@ReceiptNo,
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
				'Incoming Material',
				'NEW'
			From PartReceiptDetailBarcode a
			inner join PartReceiptHeader b on a.ReceiptId = b.Id
			inner join Item_Master mi on a.ItemCode = mi.Item_Code
			where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo

			SET @InspectionId = (select SCOPE_IDENTITY())
		end
	
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
			@QtySample,
			getdate(),
			@UserId
		)

		select @InspectionId

		commit transaction sampleTransaction
	end try
	begin catch
		rollback transaction sampleTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
GO
