SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_DataBarcode]
	@BarcodeNo varchar(100) 
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode tidak ditemukan', 16, 1)
		return
	end

	if exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode sudah direceipt!', 16, 1)
		return
	end

	declare @ReceiptId bigint = (SELECT ReceiptId fROM PartReceiptDetailBarcode WHERE BarcodeNo = @BarcodeNo)

	SELECT
		a.Id, a.ReceiptDetailId, a.ReceiptId, hd.DNNumber, hd.SupplierCode, sp.Trade_Name SupplierName,
		a.PONumber, 
		a.ItemCode,
		b.Item_Name [ItemName],
		a.BarcodeNo, a.LotNo, a.SublotNo, a.Qty,
		a.IsVerified, a.VerifiedBy, a.VerifiedDate
	FROM PartReceiptDetailBarcode a
	inner join PartReceiptHeader hd on a.ReceiptId = hd.Id
	inner join Trade_Master sp on hd.SupplierCode = sp.Trade_Code
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	WHERE a.ReceiptId = @ReceiptId
	AND a.BarcodeNo = @BarcodeNo
end
GO
