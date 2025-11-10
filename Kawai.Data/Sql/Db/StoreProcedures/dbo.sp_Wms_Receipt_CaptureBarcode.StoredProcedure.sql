SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Receipt_CaptureBarcode]
	@Id bigint 
as

select * From PartReceiptDetailBarcode where Id = @Id

declare @receiptId bigint = (select ReceiptId from PartReceiptDetailBarcode where Id = @Id)
select Id, ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, StatusReceipt, IsManual, Transport  
From PartReceiptHeader where Id = @receiptId

select 
	a.WarehouseCode, a.AreaCode, a.ItemCode, a.LotNo, 
	a.LMPreMonth, a.LMReceipt, a.LMSupply, a.LMLossReject, a.LMCurrent, a.LMInventory, 
	a.TMPreMonth, a.TMReceipt, a.TMSupply, a.TMLossReject, a.TMCurrent, a.TMInventory, 
	a.NMPreMonth, a.NMReceipt, a.NMSupply, a.NMLossReject, a.NMCurrent, a.NMInventory, 
	a.LMReason, a.TMReason, a.NMReason, a.Adjustment 
from StockMaster a 
inner join 
(
	select distinct ItemCode, LotNo from PartReceiptDetailBarcode where Id = @Id
) b on a.ItemCode = b.ItemCode and a.LotNo = b.LotNo 


select 
	a.WarehouseCode, a.AreaCode, a.AddressCode, a.BarcodeNo, a.ItemCode, a.LotNo, a.SublotNo, a.Qty, a.InventoryQty, 
	a.ExpiredDate, a.ProductionDate, a.ReceiptDate, a.Supplier, a.PrintCls, a.DisposalCls 
from StockDetail a 
inner join 
(
	select distinct ItemCode, LotNo, BarcodeNo from PartReceiptDetailBarcode where Id = @Id
) b on a.ItemCode = b.ItemCode and a.LotNo = b.LotNo and a.BarcodeNo = b.BarcodeNo
and Qty > 0


GO
