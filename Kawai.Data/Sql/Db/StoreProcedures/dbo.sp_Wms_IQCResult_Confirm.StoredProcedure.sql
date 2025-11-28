/****** Object:  StoredProcedure [dbo].[sp_Wms_IQCResult_Confirm]    Script Date: 11/28/2025 10:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [dbo].[sp_Wms_IQCResult_Confirm]
	@InspectionId bigint,
	@InspectionResult varchar(100),
	@UserId varchar(25)
as
begin
	if exists (select 1 from IQC_Inspection_Header where InspectionID = @InspectionId and InspectionResultDate is not null)
	begin
		raiserror('Data QC sudah diapprove', 16,1)
		return
	end

	declare @Source varchar(50), @ReceiptId bigint 
	select 
		@Source = Soruce, @ReceiptId = prh.Id
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	where iqch.InspectionID = @InspectionId

	/* 
		DISINI HARUS NYA ADA VALIDASI CEK, BARCODE2 YG ADA DI QC INI ADA YG LG DIPAKE GA DI TRANSAKSI KAYA MATERIAL REQUEST / CONSUME
		KHUSUS YG SOURCE NYA 'MATERIAL NG' AJA
	*/

	update IQC_Inspection_Header 
	set 
		InspectionResult = @InspectionResult, InspectionResultApproval = @UserId, InspectionResultDate = getdate(), LastUpdate = getdate() 
	where InspectionID = @InspectionId

	if @Source = 'Incoming Material'
	begin
		update sd 
		set 
			StatusReceipt = 'OK',
			Lastupdate = getdate(),
			LastUser = @UserId
		From StockDetail sd
		inner join 
		(
			select BarcodeNo, ItemCode, LotNo From PartReceiptDetailBarcode 
			where ReceiptId = @ReceiptId
		) qc on sd.BarcodeNo = qc.BarcodeNo and sd.ItemCode = qc.ItemCode and sd.LotNo = qc.LotNo
	end
	else if @Source = 'Material NG'
	begin
		update sd 
		set 
			StatusReceipt = 'NG',
			Lastupdate = getdate(),
			LastUser = @UserId
		From StockDetail sd
		inner join 
		(
			select BarcodeNo From IQC_SamplingBarcodeDetail 
			where InspectionID = @InspectionId
		) qc on sd.BarcodeNo = qc.BarcodeNo
	end


end
GO
