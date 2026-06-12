
create procedure [dbo].[sp_Wms_Robot_CaptureStockTrolley]
	@RefNo varchar(50)
as

select 
	a.RefNo, a.WarehouseCode, a.AreaCode, a.ItemCode, a.LotNo, 
	a.LMPreMonth, a.LMReceipt, a.LMSupply, a.LMLossReject, a.LMCurrent, a.LMInventory, 
	a.TMPreMonth, a.TMReceipt, a.TMSupply, a.TMLossReject, a.TMCurrent, a.TMInventory, 
	a.NMPreMonth, a.NMReceipt, a.NMSupply, a.NMLossReject, a.NMCurrent, a.NMInventory, 
	a.LMReason, a.TMReason, a.NMReason, a.Adjustment 
from StockHeader a 
where RefNo = @RefNo

select 
	a.RefNo, a.WarehouseCode, a.AreaCode, a.AddressCode, a.BarcodeNo, a.ItemCode, a.LotNo, a.SublotNo, a.Qty, a.InventoryQty, 
	a.ExpiredDate, a.ProductionDate, a.ReceiptDate, a.Supplier, a.PrintCls, a.DisposalCls, a.StatusReceipt, a.Picking_No
from StockDetail a 
where RefNo = @RefNo
and Qty > 0
