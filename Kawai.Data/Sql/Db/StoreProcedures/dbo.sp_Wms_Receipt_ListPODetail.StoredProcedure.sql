
create   procedure [dbo].[sp_Wms_Receipt_ListPODetail]
	@FactoryCode varchar(25),
	@ReceiptId bigint,
	@PONumber varchar(25),
	@SupplierCode varchar(25),
	@DateFrom datetime,
	@DateUntil datetime 
as
begin
	declare @statusReceipt varchar(20)
	if @ReceiptId is not null
	begin
		set @statusReceipt = (select StatusReceipt From PartReceiptHeader where Id = @ReceiptId)
	end
	
	IF @statusReceipt IS NULL
	BEGIN
		SET @statusReceipt = 'NEW'
	END
		
	IF ISNULL(@PONumber, '') <> 'ALL'
	BEGIN
		IF @statusReceipt = 'NEW'
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
				coalesce(f.QtyPacking, d.Number_Box, 0) [QtyPacking],
				CEILING(b.Qty / nullif(isnull(f.QtyPacking, d.Number_Box), 0)) [TotalPacking],
				rcp.NoSeri [NoSeri],
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
			 		PO_No PONumber, Item_Code ItemCode, sum(Qty) ReceiptQty 
				from Part_Receipt 
				where PO_No = @PONumber
				and (RefWMSReceiptId is null or isnull(RefWMSReceiptId, 0) <> isnull(@ReceiptId, 0))
				group by PO_No, Item_Code
			) rcpSum on a.PO_No = rcpSum.PONumber and b.Item_Code = rcpSum.ItemCode
			left join 
			(
				select * From PartReceiptDetail 
				where ReceiptId = @ReceiptId and PONumber = @PONumber
			) rcp on a.PO_No = rcp.PONumber and b.Item_Code = rcp.ItemCode
			left join vw_User us on a.Last_User = us.UserID
			WHERE 1=1 
			AND a.PO_No = @PONumber
			and isnull(a.Fix_Cls, '0') = '1'
			AND (b.Qty - isnull(rcpSum.ReceiptQty, 0)) > 0
			order by isnull(rcp.ReceiptQty, 0) desc,  b.Item_Code
		END
		ELSE
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
				coalesce(f.QtyPacking, d.Number_Box, 0) [QtyPacking],
				CEILING(b.Qty / nullif(isnull(f.QtyPacking, d.Number_Box), 0)) [TotalPacking],
				rcp.NoSeri [NoSeri],
				null [ProductionDate],
				a.Last_Update [LastUpdate],
				us.FullName [LastUser]
			FROM PurchaseOrder_Master a
			inner join PurchaseOrder_Detail b on a.PO_No = b.PO_No
			inner join 
			(
				select * From PartReceiptDetail 
				where ReceiptId = @ReceiptId and PONumber = @PONumber
			) rcp on a.PO_No = rcp.PONumber and b.Item_Code = rcp.ItemCode			
			left join trade_master c on a.Supplier_Code = c.Trade_Code
			left join Item_Master d on b.Item_Code = d.Item_Code
			left join Unit_Cls e on b.Unit_Cls = e.Unit_Cls
			left join ItemSupplierPacking f on b.Item_Code = f.ItemCode and a.Supplier_Code = f.SupplierCode
			left join 
			(
				select 
			 		PO_No PONumber, Item_Code ItemCode, sum(Qty) ReceiptQty 
				from Part_Receipt 
				where PO_No = @PONumber
				group by PO_No, Item_Code
			) rcpSum on a.PO_No = rcpSum.PONumber and b.Item_Code = rcpSum.ItemCode			
			left join vw_User us on a.Last_User = us.UserID
			WHERE 1=1 
			AND a.PO_No = @PONumber
			and isnull(a.Fix_Cls, '0') = '1'
			order by isnull(rcp.ReceiptQty, 0) desc,  b.Item_Code
		END
	END
	else 
	begin
		declare @tblReceipt table (po varchar(100), item varchar(50), totalReceipt numeric(18,9))

		IF @statusReceipt = 'NEW'
		BEGIN
			insert into @tblReceipt
			select a.PO_No, a.Item_Code, sum(a.Qty) ReceiptQty from Part_Receipt a
			inner join 
			(
				select * From PurchaseOrder_Master x
				where Delivery_Date between @DateFrom and @DateUntil and Supplier_Code = @SupplierCode
				and isnull(x.Fix_Cls, '0') = '1'
			) po on a.PO_No = po.PO_No
			where (RefWMSReceiptId is null or isnull(RefWMSReceiptId, 0) <> isnull(@ReceiptId, 0))
			group by a.PO_No, a.Item_Code

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
				coalesce(f.QtyPacking, d.Number_Box, 0) [QtyPacking],
				CEILING(b.Qty / nullif(isnull(f.QtyPacking, d.Number_Box), 0)) [TotalPacking],
				rcp.NoSeri [NoSeri],
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
			and isnull(a.Fix_Cls, '0') = '1'
			AND (b.Qty - isnull(rcpSum.totalReceipt, 0)) > 0
			order by isnull(rcp.ReceiptQty, 0) desc, a.PO_No, b.Item_Code
		END
		ELSE
		BEGIN
			insert into @tblReceipt
			select a.PO_No, a.Item_Code, sum(a.Qty) ReceiptQty from Part_Receipt a
			inner join 
			(
				select * From 
				(
					select PO_No From PurchaseOrder_Master 
					where Delivery_Date between @DateFrom and @DateUntil and Supplier_Code = @SupplierCode
					and isnull(Fix_Cls, '0') = '1'
				) x
				inner join 
				(
					select distinct PONumber, ItemCode From PartReceiptDetail where ReceiptId = @ReceiptId
				) y on x.PO_No = y.PONumber
			) po on a.PO_No = po.PO_No and a.Item_Code = po.ItemCode
			group by a.PO_No, a.Item_Code

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
				coalesce(f.QtyPacking, d.Number_Box, 0) [QtyPacking],
				CEILING(b.Qty / nullif(isnull(f.QtyPacking, d.Number_Box), 0)) [TotalPacking],
				rcp.NoSeri [NoSeri],
				null [ProductionDate],
				a.Last_Update [LastUpdate],
				us.FullName [LastUser]
			FROM 
			(
				select * from PurchaseOrder_Master 
				where 1=1
				AND Delivery_Date between @DateFrom and @DateUntil
				and Supplier_Code = @SupplierCode
				and isnull(Fix_Cls, '0') = '1'

			) a
			inner join PurchaseOrder_Detail b on a.PO_No = b.PO_No
			inner join 
			(
				select * From PartReceiptDetail where ReceiptId = @ReceiptId
			) rcp on a.PO_No = rcp.PONumber and b.Item_Code = rcp.ItemCode
			left join trade_master c on a.Supplier_Code = c.Trade_Code
			left join Item_Master d on b.Item_Code = d.Item_Code
			left join Unit_Cls e on b.Unit_Cls = e.Unit_Cls
			left join ItemSupplierPacking f on b.Item_Code = f.ItemCode and a.Supplier_Code = f.SupplierCode
			left join @tblReceipt rcpSum on a.PO_No = rcpSum.po and b.Item_Code = rcpSum.item
			left join vw_User us on a.Last_User = us.UserID
			order by isnull(rcp.ReceiptQty, 0) desc, a.PO_No, b.Item_Code
		END
	end	
end
