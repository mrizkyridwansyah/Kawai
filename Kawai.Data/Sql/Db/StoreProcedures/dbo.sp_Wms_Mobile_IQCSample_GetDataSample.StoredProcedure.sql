SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_IQCSample_GetDataSample]
	@ReceiptId bigint,
	@BarcodeNo varchar(100)
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode tidak ditemukan', 16, 1)
		return
	end

	if exists 
	(
		select 1 from IQC_SamplingBarcodeDetail dtl
		inner join IQC_Inspection_Header hd on dtl.InspectionID = hd.InspectionID 
		where dtl.BarcodeNo = @BarcodeNo and hd.Soruce = 'Incoming Material'
	)
	begin
		raiserror('Barcode sudah diinput sample', 16, 1)
		return
	end

	declare @receiptNo varchar(100) = (select ReceiptNo from PartReceiptHeader where Id = @ReceiptId)
	declare @itemCode varchar(25) = (select ItemCode from PartReceiptDetailBarcode where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo)

	if exists 
	(
		select 1 
		from IQC_Inspection_Header 
		where ReceiptNo = @receiptNo 
		and ItemCode = @itemCode 
		and Soruce = 'Incoming Material' 
		and isnull(InspectionResult, '') <> ''
	)
	begin
		raiserror('Sampel Receipt sudah diapprove!', 16, 1)
		return
	end

	SELECT
		a.Id ReceiptDetailBarcodeId, a.ReceiptDetailId, a.ReceiptId,
		smp.SamplingID SampleId, smp.InspectionID InspectionId,
		a.PONumber, 
		a.ItemCode,
		b.Item_Name [ItemName],
		a.BarcodeNo, a.LotNo, a.SublotNo, a.Qty,
		QtySample = isnull(smp.SampleQTY, 0)
	FROM PartReceiptDetailBarcode a
	LEFT JOIN 
	(
		select xx.* from IQC_SamplingBarcodeDetail xx
		INNER JOIN IQC_Inspection_Header yy on xx.InspectionID = yy.InspectionID
		WHERE yy.Soruce = 'Incoming Material'
	) smp on a.BarcodeNo = smp.BarcodeNo
	LEFT JOIN IQC_Inspection_Header hd on smp.InspectionID = hd.InspectionID
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	WHERE a.ReceiptId = @ReceiptId
	AND a.BarcodeNo = @BarcodeNo
end
GO
