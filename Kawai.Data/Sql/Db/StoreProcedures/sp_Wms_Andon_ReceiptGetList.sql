USE Kawaii
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Wms_Andon_ReceiptGetList]
AS
BEGIN
	select ph.ReceiptNo, ph.ReceiptDate, tm.Trade_Name [SupplierName], ph.DNNumber, mi.Item_Name [ItemName],coalesce(ReceiptQty,0) as ReceiptQtyUnit, coalesce(TotalPacking,0) as ReceiptQtyPack ,PH.StatusReceipt,StatusReceiptName=st.StatusReceiptName
	from PartReceiptHeader ph
	inner join PartReceiptDetail pdb on ph.Id = pdb.ReceiptId
	left join Trade_Master tm on ph.SupplierCode = tm.Trade_Code
	left join Item_Master mi on pdb.ItemCode = mi.Item_Code	
	left join VW_StatusReceipt st on st.StatusReceiptCode=ph.StatusReceipt
	where ph.StatusReceipt <> 'COMPLETE'
END
GO