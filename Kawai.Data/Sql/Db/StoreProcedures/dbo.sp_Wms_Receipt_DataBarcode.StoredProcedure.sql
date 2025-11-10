SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_DataBarcode]
	@ReceiptId bigint,
	@BarcodeNo varchar(100)
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode tidak ditemukan', 16, 1)
		return
	end

	SELECT
		a.Id, a.ReceiptDetailId, a.ReceiptId,
		a.PONumber, 
		a.ItemCode,
		b.Item_Name [ItemName],
		a.BarcodeNo, a.LotNo, a.SublotNo, a.Qty,
		a.IsVerified, a.VerifiedBy, a.VerifiedDate
	FROM PartReceiptDetailBarcode a
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	WHERE a.ReceiptId = @ReceiptId
	AND a.BarcodeNo = @BarcodeNo
end
GO
