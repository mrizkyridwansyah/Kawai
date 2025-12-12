SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Andon_ReceiptGetList]
as
begin
	select ph.ReceiptNo, ph.ReceiptDate, tm.Trade_Name [SupplierName], ph.DNNumber, mi.Item_Name [ItemName], ph.StatusReceipt 
	from PartReceiptHeader ph
	inner join PartReceiptDetail pdb on ph.Id = pdb.ReceiptId
	left join Trade_Master tm on ph.SupplierCode = tm.Trade_Code
	left join Item_Master mi on pdb.ItemCode = mi.Item_Code	
	where ph.StatusReceipt <> 'COMPLETE'
end
GO
