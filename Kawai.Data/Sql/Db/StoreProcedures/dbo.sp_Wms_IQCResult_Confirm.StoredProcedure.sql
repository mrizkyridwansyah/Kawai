SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_IQCResult_Confirm]
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

	if exists 
	(
		select 1 From IQC_Inspection_Header iqc
		inner join PartReceiptHeader prh on iqc.ReceiptNo = prh.ReceiptNo
		inner join PartReceiptDetailBarcode prdb on prh.Id = prdb.ReceiptId and iqc.ItemCode = prdb.ItemCode
		where iqc.InspectionID = @InspectionId and isnull(IsVerified, 0) = 0
	) 
	begin
		raiserror('Silahkan receive semua barcode terlebih dahulu!', 16,1)
		return
	end

	declare @Source varchar(50), @ReceiptId bigint, @ItemCode varchar(25)
	select 
		@Source = Soruce, @ReceiptId = prh.Id, @ItemCode = iqch.ItemCode
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	where iqch.InspectionID = @InspectionId

	/* 
		DISINI HARUS NYA ADA VALIDASI CEK, BARCODE2 YG ADA DI QC INI ADA YG LG DIPAKE GA DI TRANSAKSI KAYA MATERIAL REQUEST / CONSUME
		KHUSUS YG SOURCE NYA 'MATERIAL NG' AJA
	*/

	update IQC_Inspection_Header 
	set 
		InspectionResult = @InspectionResult, InspectionResultApproval = @UserId, InspectionResultDate = getdate(), LastUpdate = getdate(), StatusQC = 'CONFIRMED' 
	where InspectionID = @InspectionId

	declare @status varchar(50) = case when @InspectionResult = 'Accepted' then 'OK'  when @InspectionResult = 'Rejected' then 'NG' else 'HOLD' end

	if @Source = 'Incoming Material'
	begin		
		-- update status receipt yg dari SAMPLING SAJA! biar ketika di Material Storage ke Address diubah jadi COMPLETE dan HILANG dr ANDON RECEIVING
		update PartReceiptHeader set StatusReceipt = @status, LastUpdate = getdate(), LastUser = @UserId where Id = @ReceiptId 

		-- UPDATE Stock yg ada di receipt tersebut, QC Receiving.
		update sd 
		set 
			StatusReceipt = @status,
			Lastupdate = getdate(),
			LastUser = @UserId
		From StockDetail sd
		inner join 
		(
			select BarcodeNo, ItemCode, LotNo 
			From PartReceiptDetailBarcode 
			where ReceiptId = @ReceiptId and ItemCode = @ItemCode
		) qc on sd.BarcodeNo = qc.BarcodeNo and sd.ItemCode = qc.ItemCode and sd.LotNo = qc.LotNo
	end
	else if @Source = 'Material NG'
	begin
		-- UPDATE Stock yg ada disampling NG aja karna ini adalah NG diluar receiving
		update sd 
		set 
			StatusReceipt = @status,
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
