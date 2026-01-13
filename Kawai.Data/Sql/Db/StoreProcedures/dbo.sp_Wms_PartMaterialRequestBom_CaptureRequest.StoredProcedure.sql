SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_PartMaterialRequestBom_CaptureRequest]
	@RequestId bigint
as
begin
	select 
		pmrh.RequestID, 
		pmrh.RequestNo, 
		pmrh.RequestDate, 
		pmrh.PO_NO, 
		pmrh.Warehouse, 
		pmrh.ProductionDate,
		pmrh.ParentItem_Code,
		pmrh.RequestSetQty,
		pmrh.Status, 
		pmrh.Remarks
	From PartMaterialRequestHeader_PO pmrh 
	where pmrh.RequestID = @RequestId

	select 
		pmrd.RequestDetailID, 
		pmrd.RequestDetailNo, 
		pmrd.RequestID, 
		pmrd.AreaCode,
		pmrd.SEQ,
		pmrd.RackNumber,
		pmrd.RefNumber,
		pmrd.RequestStatusID,
		pmrd.Remarks
	From PartMaterialRequestDetail_PO pmrd 
	where pmrd.RequestID = @RequestId

	select 
		pmrid.IDSeq,
		pmrd.RequestID,
		pmrid.RequestDetailID,
		pmrid.ItemCode,
		pmrid.unit_Cls,
		pmrid.ChildRequirement_Qty,
		pmrid.Remarks
	From 
	(
		select * from PartMaterialRequestDetail_PO where RequestID = @RequestId
	) pmrd 
	inner join PartMaterialRequestItemDetail_PO pmrid on pmrd.RequestDetailID = pmrid.RequestDetailID
end
GO
