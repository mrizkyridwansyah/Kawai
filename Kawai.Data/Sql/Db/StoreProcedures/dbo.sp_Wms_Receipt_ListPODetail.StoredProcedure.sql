SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Receipt_ListPODetail]
	@ReceiptId bigint,
	@PONumber varchar(25),
	@SupplierCode varchar(25),
	@DateFrom datetime,
	@DateUntil datetime
as
begin
		
	IF ISNULL(@PONumber, '') <> 'ALL'
	BEGIN
		SELECT 
			rcp.ReceiptId [ReceiptId],
			rcp.Id [ReceiptDetailId],
			isnull(rcp.ReceiptQty, 0) [ReceiptQty],
			a.PO_No [PONumber],
			a.PO_Date [PODate],
			a.Supplier_Code [SupplierCode],
			c.Trade_Name [SupplierName],
			b.Item_Code [ItemCode],
			d.Item_Name [ItemName],
			b.Unit_Cls [UnitClsCode],
			e.Description [UnitClsName],
			b.Qty [Qty],
			isnull(rcpSum.ReceiptQty, 0) [TotalReceiptQty],
			(b.Qty - isnull(rcpSum.ReceiptQty, 0)) [RemainingQty],
			isnull(f.QtyPacking, 0) [QtyPacking],
			b.Qty / nullif(f.QtyPacking, 0) [TotalPacking],
			'' [NoSeri],
			null [ProductionDate],
			a.Last_Update [LastUpdate],
			us.FullName [LastUser]
		FROM PurchaseOrder_Master a
		inner join PurchaseOrder_Detail b on a.PO_No = b.PO_No
		left join trade_master c on a.Supplier_Code = c.Trade_Code
		left join Item_Master d on b.Item_Code = d.Item_Code
		left join Unit_Cls e on b.Unit_Cls = e.Unit_Cls
		left join ItemSupplierPacking f on b.Item_Code = f.ItemCode and a.Supplier_Code = f.SupplierCode
		left join 
		(
			select 
				PONumber, ItemCode, sum(ReceiptQty) ReceiptQty 
			from PartReceiptDetail 
			where ReceiptId <> isnull(@ReceiptId, 0) 
			and PONumber = @PONumber
			group by PONumber, ItemCode
		) rcpSum on a.PO_No = rcpSum.PONumber and b.Item_Code = rcpSum.ItemCode
		left join 
		(
			select * From PartReceiptDetail 
			where ReceiptId = @ReceiptId and PONumber = @PONumber
		) rcp on a.PO_No = rcp.PONumber and b.Item_Code = rcp.ItemCode
		left join vw_User us on a.Last_User = us.UserID
		WHERE 1=1 
		AND a.PO_No = @PONumber
		AND (b.Qty - isnull(rcpSum.ReceiptQty, 0)) > 0
	END
	else 
	begin
		declare @tblReceipt table (po varchar(100), item varchar(50), totalReceipt numeric(18,9))
		insert into @tblReceipt
		select PONumber, ItemCode, sum(ReceiptQty) ReceiptQty from PartReceiptDetail a
		inner join 
		(
			select * From PurchaseOrder_Master x
			where Delivery_Date between @DateFrom and @DateUntil and Supplier_Code = @SupplierCode
		) po on a.PONumber = po.PO_No
		where ReceiptId <> isnull(@ReceiptId, 0) 
		group by PONumber, ItemCode

		SELECT 
			rcp.ReceiptId [ReceiptId],
			rcp.Id [ReceiptDetailId],			
			isnull(rcp.ReceiptQty, 0) [ReceiptQty],
			a.PO_No [PONumber],
			a.PO_Date [PODate],
			a.Supplier_Code [SupplierCode],
			c.Trade_Name [SupplierName],
			b.Item_Code [ItemCode],
			d.Item_Name [ItemName],
			b.Unit_Cls [UnitClsCode],
			e.Description [UnitClsName],
			b.Qty [Qty],
			isnull(rcpSum.totalReceipt, 0) [TotalReceiptQty],
			(b.Qty - isnull(rcpSum.totalReceipt, 0)) [RemainingQty],
			isnull(f.QtyPacking, 0) [QtyPacking],
			b.Qty / nullif(f.QtyPacking, 0) [TotalPacking],
			'' [NoSeri],
			null [ProductionDate],
			a.Last_Update [LastUpdate],
			us.FullName [LastUser]
		FROM PurchaseOrder_Master a
		inner join PurchaseOrder_Detail b on a.PO_No = b.PO_No
		inner join 
		(
			select WarehouseCode From vw_WarehouseLine where FactoryCode = @FactoryCode
		) y on a.WHTo = y.WarehouseCode
		left join trade_master c on a.Supplier_Code = c.Trade_Code
		left join Item_Master d on b.Item_Code = d.Item_Code
		left join Unit_Cls e on b.Unit_Cls = e.Unit_Cls
		left join ItemSupplierPacking f on b.Item_Code = f.ItemCode and a.Supplier_Code = f.SupplierCode
		left join @tblReceipt rcpSum on a.PO_No = rcpSum.po and b.Item_Code = rcpSum.item
		left join 
		(
			select * From PartReceiptDetail where ReceiptId = @ReceiptId
		) rcp on a.PO_No = rcp.PONumber and b.Item_Code = rcp.ItemCode
		left join vw_User us on a.Last_User = us.UserID
		WHERE 1=1 
		AND a.Delivery_Date between @DateFrom and @DateUntil
		and a.Supplier_Code = @SupplierCode
	end	
end
GO
