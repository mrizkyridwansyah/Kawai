SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Mobile_SupplySubcon_Capture]
	@RequestNo varchar(100),
	@ItemCode varchar(25)
as
begin
	declare @RequestDetailID bigint, @PickingNo varchar(100)
	select @RequestDetailID = RequestDetailID, @PickingNo = RefNumber 
	from PartMaterialRequestDetail_PO where RequestDetailNo = @RequestNo
	
	select 
		dtl.AreaCode ItemClassification, dtl.RackNumber, dtl.RefNumber, dtl.RequestStatusID, 
		idtl.ChildRequirement_Qty PlanQty, isnull(scan.TotalScan, 0) TotalScan
	From PartMaterialRequestItemDetail_PO idtl	
	inner join PartMaterialRequestDetail_PO dtl on idtl.RequestDetailID = dtl.RequestDetailID
	left join 
	(
		select IDSeq, ItemCode, sum(qty) TotalScan From PartMaterialRequestItemDetailScan_PO
		group by IDSeq, ItemCode
	) scan on idtl.IDSeq = scan.IDSeq and idtl.ItemCode = scan.ItemCode
	where dtl.RefNumber = @PickingNo and idtl.ItemCode = @ItemCode

	select 
		scan.FromRefNo, scan.FromWarehouseCode, scan.FromAreaCode, scan.FromAddressCode, 
		scan.ToRefNo, scan.ToWarehouseCode, scan.ToAreaCode, scan.ToAddressCode, 
		scan.BarcodeNo, scan.ItemCode, scan.LotNo, scan.Qty, scan.BarcodeNoOriginal
	From PartMaterialRequestItemDetailScan_PO scan
	inner join PartMaterialRequestItemDetail_PO idtl on scan.IDSeq = idtl.IDSeq
	where idtl.RequestDetailID = @RequestDetailID and idtl.ItemCode = @ItemCode
end
GO
