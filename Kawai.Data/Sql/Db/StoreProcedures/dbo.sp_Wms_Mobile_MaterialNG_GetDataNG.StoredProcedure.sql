SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_MaterialNG_GetDataNG]
	@BarcodeNo varchar(100) 
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode tidak ditemukan', 16, 1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and StatusReceipt = 'OK')
	begin
		raiserror('Status Barcode belum OK', 16, 1)
		return
	end

	SELECT
		smp.SamplingID SampleId, smp.InspectionID InspectionId,
		c.SupplierCode,
		sp.Trade_Name SupplierName,
		a.ItemCode,
		b.Item_Name [ItemName],
		a.BarcodeNo, a.LotNo, a.SublotNo, a.Qty,
		QtyNG = isnull(smp.SampleQTY, 0)
	FROM PartReceiptDetailBarcode a
	INNER JOIN PartReceiptHeader c on a.ReceiptId = c.Id
	LEFT JOIN Trade_Master sp on c.SupplierCode = sp.Trade_Code
	LEFT JOIN 
	(
		select xx.* from IQC_SamplingBarcodeDetail xx
		INNER JOIN IQC_Inspection_Header yy on xx.InspectionID = yy.InspectionID
		WHERE yy.Soruce = 'Material NG'
	) smp on a.BarcodeNo = smp.BarcodeNo
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	WHERE a.BarcodeNo = @BarcodeNo
end
GO
