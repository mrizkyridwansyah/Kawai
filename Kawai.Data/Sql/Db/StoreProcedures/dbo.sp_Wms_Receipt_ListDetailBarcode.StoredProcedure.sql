SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_ListDetailBarcode]
	@ReceiptId bigint
as
begin
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
end
GO
