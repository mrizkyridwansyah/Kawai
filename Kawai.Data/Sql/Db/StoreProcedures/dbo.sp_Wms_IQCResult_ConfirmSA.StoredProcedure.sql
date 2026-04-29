CREATE   procedure [dbo].[sp_Wms_IQCResult_ConfirmSA]
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

	update IQC_Inspection_Header 
	set 
		InspectionResult = @InspectionResult, InspectionResultApproval = @UserId, InspectionResultDate = getdate(), LastUpdate = getdate(), StatusQC = 'PENDING-SA' 
	where InspectionID = @InspectionId
end
