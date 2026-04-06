SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Receipt_ListClaimDetail]
	@FactoryCode varchar(25),
	@ReceiptId bigint,
	@ClaimNumber varchar(25),
	@SupplierCode varchar(25),
	@DateFrom datetime,
	@DateUntil datetime 
as
begin
		
	IF ISNULL(@ClaimNumber, '') <> 'ALL'
	BEGIN
		SELECT 
			rcp.ReceiptId [ReceiptId],
			rcp.Id [ReceiptDetailId],
			isnull(rcp.ReceiptQty, 0) [ReceiptQty],
			a.ClaimNo [PONumber],
			a.ClaimDate [PODate],
			a.SupplierCode [SupplierCode],
			c.Trade_Name [SupplierName],
			b.ItemCode [ItemCode],
			d.Item_Name [ItemName],
			d.Unit_Cls  [UnitClsCode],
			e.Description [UnitClsName],
			b.QtyNG [Qty],
			isnull(rcpSum.ReceiptQty, 0) [TotalReceiptQty],
			(b.QtyNG - isnull(rcpSum.ReceiptQty, 0)) [RemainingQty],
			coalesce(f.QtyPacking, d.Number_Box, 0) [QtyPacking],
			CEILING(b.QtyNG / nullif(isnull(f.QtyPacking, d.Number_Box), 0)) [TotalPacking],
			'' [NoSeri],
			null [ProductionDate],
			a.LastUpdate [LastUpdate],
			us.FullName [LastUser]
		FROM MaterialNGClaimHeader a
		inner join MaterialNGClaimDetail b on a.ClaimID = b.ClaimID
		left join trade_master c on a.SupplierCode = c.Trade_Code
		left join Item_Master d on b.ItemCode = d.Item_Code
		left join Unit_Cls e on d.Unit_Cls = e.Unit_Cls
		left join ItemSupplierPacking f on b.ItemCode = f.ItemCode and a.SupplierCode = f.SupplierCode
		left join 
		(
			select 
				PONumber, ItemCode, sum(ReceiptQty) ReceiptQty 
			from PartReceiptDetail 
			where ReceiptId <> isnull(@ReceiptId, 0) 
			and PONumber = @ClaimNumber
			group by PONumber, ItemCode
		) rcpSum on a.ClaimNo = rcpSum.PONumber and b.ItemCode = rcpSum.ItemCode
		left join 
		(
			select * From PartReceiptDetail 
			where ReceiptId = @ReceiptId and PONumber = @ClaimNumber
		) rcp on a.ClaimNo = rcp.PONumber and b.ItemCode = rcp.ItemCode
		left join vw_User us on a.LastUser = us.UserID
		WHERE 1=1 
		AND a.ClaimNo = @ClaimNumber
		AND (b.QtyNG - isnull(rcpSum.ReceiptQty, 0)) > 0
		
	END
	 
end
GO
